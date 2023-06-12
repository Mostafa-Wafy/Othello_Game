using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI uppertext;

    [SerializeField]
    private TextMeshProUGUI BScoreUp;

    [SerializeField]
    private TextMeshProUGUI WScoreUp;

    [SerializeField]
    private TextMeshProUGUI WinnerText;

    [SerializeField]
    private RawImage overlayImage;

    [SerializeField]
    private RectTransform playAgainBtn;

    [SerializeField]
    private RectTransform returnToMainBtn;

    public void SetPlayerText(Player Player){

        if(Player == Player.Black){

            uppertext.text = "Black's Turn <sprite name=DiskBlackUp>";
        }

        if(Player == Player.White){

            uppertext.text = "White's Turn <sprite name=DiskWhiteUp>";
        }

    }

    public void SetWhiteScoreText(int score){
        WScoreUp.text = $"<sprite name=DiskWhiteUp> {score}";
    }
    
    public void SetBlackScoreText(int score){
        BScoreUp.text = $"<sprite name=DiskBlackUp> {score}";
    }

    public void SetSkippedText(Player SkippedPlayerTurn){

        if(SkippedPlayerTurn == Player.Black){

            uppertext.text = "Black can't move <sprite name=DiskBlackUp>";
        }

        if(SkippedPlayerTurn == Player.White){

            uppertext.text = "White can't move <sprite name=DiskWhiteUp>";
        }

    }

    public IEnumerator AnimateUpperText(){

        uppertext.transform.LeanScale(Vector3.one *1.2f, 0.25f).setLoopPingPong(4);
        yield return new WaitForSeconds(2);

    }

    public void SetUpperText(string msg){

        uppertext.text = msg;

    }

    public IEnumerator ScaleDown(RectTransform rec){

        rec.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rec.gameObject.SetActive(false);

    }

    public IEnumerator ScaleUp(RectTransform rec){

        rec.gameObject.SetActive(true);
        rec.localScale = Vector3.one;
        rec.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);

    }

    public IEnumerator ShowScoreText(){
        
        yield return ScaleUp(BScoreUp.rectTransform);
        yield return ScaleUp(WScoreUp.rectTransform);

    }

    


    private IEnumerator ShowOverlay(){

        overlayImage.gameObject.SetActive(true);
        overlayImage.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay(){

        overlayImage.rectTransform.LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        overlayImage.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoreDown(){

        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Player winner){

        switch(winner){

            case Player.Black:
                WinnerText.text = "BLACK WON";
                break;
            case Player.White:
                WinnerText.text = "WHITE WON";
                break;
            case Player.None:
                WinnerText.text = "IT'S A TIE";
                break;
        }
    }

    public IEnumerator ShowEndGameScreen(){

        yield return ShowOverlay();
        yield return MoveScoreDown();
        yield return ScaleUp(WinnerText.rectTransform);
        yield return ScaleUp(playAgainBtn);
        yield return ScaleUp(returnToMainBtn);
    }

    public IEnumerator HideEndGameScreen(){

        StartCoroutine(ScaleDown(WinnerText.rectTransform));
        StartCoroutine(ScaleDown(BScoreUp.rectTransform));
        StartCoroutine(ScaleDown(WScoreUp.rectTransform));
        StartCoroutine(ScaleDown(playAgainBtn));
        StartCoroutine(ScaleDown(returnToMainBtn));

        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }

}
