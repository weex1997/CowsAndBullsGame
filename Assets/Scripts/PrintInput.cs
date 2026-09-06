using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrintInput : MonoBehaviour
{
    [SerializeField] GameObject _printInput;
    [SerializeField] char[] inputNum;
    [SerializeField] GameObject cowPrefab;
    [SerializeField] GameObject bullPrefab;
    public string _attemptString;
    public int _currentAttempt;
    public int cows;
    public int bulls;

    // Start is called before the first frame update
    void Start()
    {
        //this if for bugs coming for shere when clone this game object the start running in the clone and the value change
        if (transform.parent.name == "Content")
        {
            transform.GetChild(0).GetComponent<TMP_Text>().text = _currentAttempt.ToString();
            inputNum = _attemptString.ToCharArray();

            for (int i = 0; i < GameManager.Instance.Lenght; i++)
            {
                GameObject printInput = Instantiate(_printInput, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                printInput.transform.SetParent(transform.GetChild(1), false);
                printInput.transform.localScale = new Vector3(1, 1, 1);
                printInput.GetComponentInChildren<TMP_Text>().text = inputNum[i].ToString();
                printInput.name = printInput.GetComponentInChildren<TMP_Text>().text;

            }


            Invoke("NumberOfCowsAndBulls", 0.1f);


            //delGuess
            keyboardManager.Instance.DeleteALLButton();
        }
    }
    void NumberOfCowsAndBulls()
    {
        //how many cows and bulls
        for (int x = 0; x < GameManager.Instance.Lenght; x++)
        {
            if (GameManager.Instance.hiddenNumberList[x] == System.Convert.ToInt32(inputNum[x].ToString()))
            {
                bulls++;
            }
            else if (GameManager.Instance.hiddenNumberList.Contains(System.Convert.ToInt32(inputNum[x].ToString())))
            {
                cows++;
            }
        }

        if (cows > 0)
        {
            GameObject cowsObject = Instantiate(cowPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            cowsObject.transform.SetParent(transform.GetChild(2));
            cowsObject.transform.localScale = new Vector3(1, 1, 1);
            cowsObject.transform.localPosition = new Vector3(0, 0, 0);
            cowsObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "x" + cows.ToString();
        }
        if (bulls > 0)
        {
            GameObject bullsObject = Instantiate(bullPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            bullsObject.transform.SetParent(transform.GetChild(2));
            bullsObject.transform.localScale = new Vector3(1, 1, 1);
            bullsObject.transform.localPosition = new Vector3(0, 0, 0);
            bullsObject.transform.GetChild(0).GetComponent<TMP_Text>().text = "x" + bulls.ToString();

        }

        //win
        if (bulls == GameManager.Instance.Lenght)
        {
            GameManager.Instance.itsWinning = true;
        }
        //lose
        else if (GameManager.Instance.totalAttempts <= 0 && !GameManager.Instance.itsWinning)
        {
            GameManager.Instance.itsLosing = true;
        }

        Debug.Log(cows + " Cows And " + bulls + " Bulls");

    }

}
