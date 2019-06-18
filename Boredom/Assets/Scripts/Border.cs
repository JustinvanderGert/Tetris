using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Border : MonoBehaviour
{
    public List<GameObject> PlacedBlocks;
    public Transform FurthestBlock;

    GameManager gameManager;


    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        FurthestBlock = transform;
    }

    void Update()
    {
        RaycastHit RHit;
        Debug.DrawRay(transform.position, Vector3.right, Color.red);

        for (int i = 0; i < 10; i++)
        {
            if (Physics.Raycast(FurthestBlock.position, Vector3.right, out RHit, 0.75f) && RHit.collider.gameObject.GetComponentInParent<Pieces>().Placed)
            {
                //RHit.collider.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                PlacedBlocks.Add(RHit.collider.gameObject);
                FurthestBlock = RHit.collider.transform;
            }
            else { break; }
        }

        if (PlacedBlocks.Count < 10)
        {
            PlacedBlocks.Clear();
            FurthestBlock = transform;
        }

        if (PlacedBlocks.Count == 10)
        {
            foreach(GameObject Block in PlacedBlocks)
            {
                Pieces piecesTemp = Block.GetComponentInParent<Pieces>();

                Destroy(Block);
                piecesTemp.StopEverything();
                piecesTemp.Refall = true;
                piecesTemp.LostOne = true;
                StartCoroutine(piecesTemp.RecheckBlocks(0));
                StartCoroutine(piecesTemp.RecheckBlocks(0.5f));
                StartCoroutine(piecesTemp.RecheckBlocks(1));
                StartCoroutine(gameManager.ReCheckAll(0.2f));
                StartCoroutine(gameManager.ReCheckAll(0.2f + piecesTemp.DropSpeed));
                gameManager.Score += 10;
            }
            FurthestBlock = transform;
            PlacedBlocks.Clear();
        }
    }
}