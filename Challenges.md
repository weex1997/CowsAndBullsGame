# Technical Challenges

Problems I hit building Cows & Bulls over three months as a solo developer, and how I handled them. Each entry gives the symptom, the diagnosis, the alternatives I weighed, and what it taught me.

---

## 1. Making guesses feel printed rather than listed

**Context**
The whole design rests on one metaphor: a physical invoice printer where each guess prints as a paper slip sliding out of the machine, pushing earlier slips upward. If that motion felt like a list gaining a row, the game lost the thing that made it distinctive.

**Alternatives considered**

| Approach | Outcome |
|---|---|
| `ScrollRect` with a vertical layout group | ❌ Rejected — rows snap into place instantly; nothing emerges from anywhere |
| Animate each row's own position on spawn | ❌ Rejected — every row below must move too, so each needs its own tween, and they drift out of sync as the list grows |
| Offset the whole container, then tween it back | ✅ Chosen |

**Implementation**
At the moment a new row is instantiated, the container is snapped downward by roughly one row height, then tweened back to its resting position:

```csharp
ScrollView.transform.position = new Vector3(x, spwanPoint.position.y - 0.6f, z);
LeanTween.moveY(ScrollView, spwanPoint.position.y, 0.4f);
```

The new slip therefore appears to slide out of the printer while everything above it moves as a single rigid sheet. The row itself is a prefab instantiated under the scroll content, with the guess string and attempt number injected before its own `Start` runs, so the cow and bull icons animate in as part of the same beat rather than a frame later.

**Accepted trade-off**
The `-0.6f` offset is a hard-coded world-space value tied to the prefab's height. Resizing the row prefab silently breaks the alignment, because nothing derives the offset from the prefab itself. Reading the row height at runtime would remove the coupling — I left it because the prefab was stable, which is a reasonable call and still a debt.

**Lesson**
Moving one parent is cheaper and more reliable than moving N children. When several elements need to appear to move together, animate the thing they share.

---

## 2. Sharing a scrolling board as one image

**Symptom**
Players wanted to share their finished game, but the guess history lives in a scrolling region. A screenshot captures the viewport, which almost never contains the whole game — and the whole game is the interesting part.

**Alternatives considered**

| Approach | Outcome |
|---|---|
| Screenshot the visible area | ❌ Rejected — captures a fragment |
| Programmatically scroll and stitch several captures | ❌ Rejected — seams, timing, and a visible scroll animation the player didn't ask for |
| Compose the full history into a dedicated camera and render once | ✅ Chosen |

**Implementation**
The complete guess history is laid out in a camera view sized to the content rather than the screen, captured in a single render, and passed to `NativeShare` with a message. The share sheet is the platform's own, so the flow behaves natively on both iOS and Android without separate code paths.

**Lesson**
When the viewport is smaller than the content, capture from a camera framed on the content — not from the screen.

---

## 3. Guess handling grew into one method that did everything

**Symptom**
Submitting a guess required checking that every input slot was filled, assembling the digits into a string, resetting the cursor, decrementing the score and attempt counter, spawning the receipt row, and playing two sound effects. All of it ended up inside a single method.

**Diagnosis**
`CheckCowsAndBulls()` in `GameManager` came to own input validation, scoring, UI animation, prefab instantiation, and audio at once. The guard against double submission is a bare boolean:

```csharp
if (cheackInputEmpty == 0 && delayGuessButton == false)
```

`delayGuessButton` is set true on entry and cleared later by animation timing, which means the input cooldown is tied to how long a tween happens to take rather than to when the game is actually ready for the next guess. It works, but the two are only related by coincidence.

**What I would do differently**
Pull guess evaluation out as a pure function — one that takes a guess and a hidden number and returns cow and bull counts, with no `MonoBehaviour` dependency and no knowledge of the UI. That function would be directly unit-testable, and the rest of the method would shrink to orchestration.

**Lesson**
Game logic that can be written as a pure function should live outside `MonoBehaviour`. Everything in this codebase that resisted testing is logic that got tangled with presentation, and untangling it is almost always mechanical once you see the seam.

---

## 4. Localizing by array index

**Symptom**
Language switching works correctly with two languages, but it is fragile by construction rather than by accident.

**Diagnosis**
`LocalizationManager` maps dropdown positions directly onto locale array positions:

```csharp
case 0: LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
case 1: LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
```

`AvailableLocales.Locales` carries no ordering guarantee. Adding a third language, or any change in how locales load, can silently remap what each dropdown entry selects. The failure mode is the worst kind: nothing throws, nothing logs, and the code still looks correct in review — the app just quietly switches to the wrong language.

**Fix**
Select by locale identifier (`"ar"`, `"en"`) instead of position, so the mapping is explicit and independent of load order.

**Lesson**
Whenever a UI index is used as a key into a collection the framework owns, that framework's ordering becomes an undocumented dependency of your code.

---

## Known limitations

| Area | Current state | Better approach |
|---|---|---|
| Save "encryption" | XOR against a fixed four-character key, currently disabled | Call it obfuscation — accurate, and acceptable for a game with no purchasable state |
| Locale selection | By array index | By locale code |
| Receipt offset | Hard-coded `-0.6f` | Derived from prefab height at runtime |
| Guess evaluation | Embedded in `MonoBehaviour` | Pure function with unit tests |

## What three months solo taught me

Every integration — PlayFab, NativeShare, Unity Localization, DOTween — took far longer to wire up correctly than to actually use. Choosing them cost hours. Getting each one's initialization order, platform differences, and error paths right cost days.

Estimating for integration rather than for features is the planning lesson I took from this project, and it's held up on everything I've built since.
