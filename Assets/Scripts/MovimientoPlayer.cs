using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovimientoPlayer : MonoBehaviour
{
   private BoxCollider2D bodyBox;
   private RaycastHit2D hit;
   private Vector3 movePlyr;

   public Animator animator;

   public float speed;

   private void Start(){
    bodyBox = GetComponent<BoxCollider2D>();
   }

   //movPer
   //propio de unity
   void FixedUpdate(){
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        movePlyr = new Vector3(hor * speed, ver * speed, 0);

        animator.SetFloat("Horizontal",hor);
        animator.SetFloat("Vertical", ver);


//comprobar si el colaider colisiona
        hit = Physics2D.BoxCast(transform.position,bodyBox.size,0,new Vector2(0,movePlyr.y), Mathf.Abs(movePlyr.y), LayerMask.GetMask("Player","Blocking"));
        if (hit.collider == null){
            transform.Translate(0, movePlyr.y, 0);
         }

      hit = Physics2D.BoxCast(transform.position,bodyBox.size,0,new Vector2(movePlyr.x,0), Mathf.Abs(movePlyr.x), LayerMask.GetMask("Player","Blocking"));
        if (hit.collider == null){
            transform.Translate(movePlyr.x, 0, 0);
         }




   }
}
