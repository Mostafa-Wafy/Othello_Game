using UnityEngine;

public class DiskScript : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private Player up;

    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Twitch () {

        animator.Play("Twitch");
        
    }


    public void Flip() {

        if (up == Player.Black){

            animator.Play("Blacktowhite");
            up = Player.White;
        }
        else {

            animator.Play("Whitetoblack");
            up = Player.Black;

        }
        
    }

    
}
