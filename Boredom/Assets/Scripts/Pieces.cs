using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pieces : MonoBehaviour
{
    public List<GameObject> BlockParts;
    public float DropSpeed;
    public int GridSize;
    public ParticleSystem Landing;

    public bool Bottom;
    public bool Placed;
    public bool AllClear;
    public bool Square;
    public bool Refall;
    public bool LostOne;

    public bool LMoveBlock;
    public bool RMoveBlock;
    public bool LTurnBlock;
    public bool RTurnBlock;
    public bool FirstPlace;

    float Xpos;
    float Ypos;
    float Zpos;
    
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        DropSpeed = 1;
        GridSize = 12;
        AllClear = true;
        LTurnBlock = false;
        RTurnBlock = false;
        FirstPlace = false;
        StartCoroutine(Descend(DropSpeed));
        Landing = GetComponent<ParticleSystem>();

        if (Bottom) { FirstPlace = true; }
    }

    void Update()
    {
        Xpos = transform.position.x;
        Ypos = transform.position.y;
        Zpos = transform.position.z;

        if(Placed && transform.position.y == 12)
        {
            SceneManager.LoadScene(0);
        }


        if(BlockParts.Count <= 0) { gameManager.PlacedBlocks.Remove(this.gameObject); Destroy(this.gameObject); }
        if (LostOne) { StopCoroutine(Descend(0)); }

        if (!Placed)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !LTurnBlock && !Square && !Placed)
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<GhostBlocks>() != null && child.GetComponent<GhostBlocks>().Blocked && child.GetComponent<GhostBlocks>().LMove)
                    {
                        LTurnBlock = true;
                        break;
                    }
                }
                if (!LTurnBlock)
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                LTurnBlock = false;
            }
            if (Input.GetKeyDown(KeyCode.E) && !RTurnBlock && !Square && !Placed)
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<GhostBlocks>() != null && child.GetComponent<GhostBlocks>().Blocked && child.GetComponent<GhostBlocks>().RMove)
                    {
                        RTurnBlock = true;
                        break;
                    }
                }
                if (!RTurnBlock)
                {
                    transform.Rotate(0, 0, -90);
                }
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                RTurnBlock = false;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                foreach(Transform child in transform)
                {
                    if (child.GetComponent<Blocks>() != null && child.GetComponent<Blocks>().LBlocked)
                    {
                        LMoveBlock = true;
                        break;
                    }
                }
                if (!LMoveBlock)
                {
                    transform.position += new Vector3(-1, 0, 0);
                }
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                LMoveBlock = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<Blocks>() != null && child.GetComponent<Blocks>().RBlocked)
                    {
                        RMoveBlock = true;
                        break;
                    }
                }
                if (!RMoveBlock)
                {
                    transform.position += new Vector3(1, 0, 0);
                }
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                RMoveBlock = false;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                    DropSpeed = 0.2f;
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                    DropSpeed = 1;
            }
        }

        if (!AllClear)
        {
            StopCoroutine(Descend(0));
            Placed = true;

            bool done = false;

            if (done)
            {
                Landing.Play();
                done = true;
            }
        }
    }

    public void StopEverything()
    {
        StopAllCoroutines();
    }

    public IEnumerator RecheckBlocks(float time)
    {
        yield return new WaitForSeconds(time);
        Refall = true;
        AllClear = true;

        for (int i = 0; i < BlockParts.Count; i++)
        {
            if (BlockParts[i] == null)
            {
                BlockParts.RemoveAt(i);
                continue;
            }
            StartCoroutine(BlockParts[i].GetComponent<Blocks>().ReCheck());
        }
    }

    public IEnumerator Descend(float Time)
    {
        yield return new WaitForSeconds(Time);

        if (!Placed && !Bottom && AllClear)
        {
            transform.position = new Vector3(Xpos, Ypos - 1, Zpos);

            StartCoroutine(Descend(DropSpeed));
        }
        else
        {
            if (!FirstPlace && Placed)
            {
                gameManager.PlacedBlocks.Add(this.gameObject);
                GetComponent<ParticleSystem>().Play();
                FirstPlace = true;
            }
            StopCoroutine(Descend(0));
            Placed = true;
        }

        if (Refall && !Bottom && AllClear)
        {
            transform.position = new Vector3(Xpos, Ypos - 1, Zpos);

            StartCoroutine(RecheckBlocks(0.2f));
        }
    }
}