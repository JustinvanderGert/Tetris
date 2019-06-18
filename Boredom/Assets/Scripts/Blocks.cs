using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    Pieces piecesScript;

    public int PieceType;
    public bool SeesOne;
    public bool LBlocked;
    public bool RBlocked;
    public bool IsSelf;
    public bool Sighted;
    public ParticleSystem Landing;

    float Xpos;
    float Ypos;
    float Zpos;
    

    void Awake()
    {
        Landing = GetComponentInParent<ParticleSystem>();
        piecesScript = gameObject.GetComponentInParent<Pieces>();
    }

    void Update()
    {
        Xpos = transform.position.x;
        Ypos = transform.position.y;
        Zpos = transform.position.z;

        RaycastHit BottomHit;
        Debug.DrawRay(transform.position, -Vector3.up, Color.red);

        if (Physics.Raycast(transform.position, -Vector3.up, out BottomHit, 0.75f) && BottomHit.collider.gameObject.GetComponentInParent<Pieces>().Placed)
        {
            IsSelf = false;
            Sighted = true;

            foreach (GameObject Block in piecesScript.BlockParts)
            {
                if (BottomHit.collider.gameObject == Block)
                {
                    IsSelf = true;
                }
            }
        }
        else { Sighted = false; }

        if (!IsSelf && Sighted)
        {
            SeesOne = true;
        }
        else
        {
            SeesOne = false;
        }

        if (SeesOne)
        {
            piecesScript.AllClear = false;
            piecesScript.Refall = false;
        }

        RaycastHit LHit;
        Debug.DrawRay(transform.position, -Vector3.right, Color.red);

        if (Physics.Raycast(transform.position, -Vector3.right, out LHit, 0.75f) && LHit.collider.gameObject.GetComponentInParent<Pieces>().Placed && !gameObject.GetComponentInParent<Pieces>().Placed)
        {
            LBlocked = true;
        }
        else
        { LBlocked = false; }

        RaycastHit RHit;
        Debug.DrawRay(transform.position, Vector3.right, Color.red);

        if (Physics.Raycast(transform.position, Vector3.right, out RHit, 0.75f) && RHit.collider.gameObject.GetComponentInParent<Pieces>().Placed && !gameObject.GetComponentInParent<Pieces>().Placed)
        {
            RBlocked = true;
        }
        else { RBlocked = false; }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<Pieces>().Placed && piecesScript.Placed == true)
        {
            other.gameObject.GetComponent<GhostBlocks>().Blocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<Pieces>().Placed && piecesScript.Placed == true)
        {
            other.gameObject.GetComponent<GhostBlocks>().Blocked = false;
        }
    }

    public IEnumerator ReCheck()
    {
        yield return new WaitForSeconds(0f);
        SeesOne = false;
        IsSelf = false;
        piecesScript.Refall = true;

        if (!piecesScript.LostOne)
        {
            RaycastHit BottomHit;
            Debug.DrawRay(transform.position, -Vector3.up, Color.red);

            if (Physics.Raycast(transform.position, -Vector3.up, out BottomHit, 0.75f) && BottomHit.collider.gameObject.GetComponentInParent<Pieces>().Placed)
            {
                Sighted = true;

                foreach (GameObject Block in piecesScript.BlockParts)
                {
                    if (BottomHit.collider.gameObject == Block)
                    {
                        IsSelf = true;
                    }
                }
            }
            else { Sighted = false; }

            if (!IsSelf && Sighted)
            {
                SeesOne = true;
            }
            else if(!Sighted)
            {
                SeesOne = false;
                StartCoroutine(piecesScript.Descend(0));
            }

            if (SeesOne)
            {
                piecesScript.Refall = false;
            }
            if(SeesOne && !IsSelf)
            {
                Landing.Play();
            }
        }
        else
        {
            RaycastHit BottomHit2;
            Debug.DrawRay(transform.position, -Vector3.up, Color.red);

            if (Physics.Raycast(transform.position, -Vector3.up, out BottomHit2, 0.75f))
            {
                StopCoroutine(piecesScript.Descend(0));
                piecesScript.AllClear = false;
            }
            else
            {
                piecesScript.AllClear = true;
                transform.position = new Vector3(Xpos, Ypos - 1, Zpos);
                StartCoroutine(ReCheck());
            }
        }
    }
}