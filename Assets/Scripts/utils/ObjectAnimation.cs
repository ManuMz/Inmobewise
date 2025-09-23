using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectAnimation : MonoBehaviour
{
    private float touchTime;    //Conteo de tiempo para saber si quiere reproducir animación
    private GameObject objectTouchedAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            TouchOnAnim();
        }
    }

    public void TouchOnAnim()
    {
        if (this.transform.GetChild(0).GetComponent<Animator>())
        {

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray;
                RaycastHit[] hits;

                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                hits = Physics.RaycastAll(ray);

                System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.transform.IsChildOf(this.transform))
                    {
                        if (hits[i].collider.transform != this.transform)
                        {
                            touchTime = Time.time;
                            objectTouchedAnim = hits[i].collider.transform.parent.gameObject;
                            break;
                        }
                    }
                }
            }

            if (objectTouchedAnim)
            {
                if (touchTime + 0.5f > Time.time)
                {
                    touchTime = 0;
                    this.transform.GetChild(0).GetComponent<Animator>().SetTrigger(objectTouchedAnim.name);
                    objectTouchedAnim = null;
                }
            }
        }
    }
}
