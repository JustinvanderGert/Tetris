using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> PlacedBlocks;
    public List<GameObject> Pieces;
    public GameObject Spawner;
    public GameObject SavedPiece;
    public int Score;
    public Text ScoreText;

    GameObject ControlledPiece;
    Pieces tempPieces;

    void Start()
    {
        int i = Random.Range(0, Pieces.Count);
        ControlledPiece = Instantiate(Pieces[i], Spawner.transform.position, Spawner.transform.rotation);
        tempPieces = ControlledPiece.GetComponent<Pieces>();
        ScoreText.text = "Score: " + Score;
    }
    
    void Update()
    {
        ScoreText.text = "Score: " + Score;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.F) && SavedPiece == null)
        {
            SavedPiece = ControlledPiece;
            tempPieces.Placed = true;
            ControlledPiece.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.F) && SavedPiece != null)
        {
            Destroy(ControlledPiece);
            ControlledPiece = SavedPiece;
            tempPieces = ControlledPiece.GetComponent<Pieces>();
            SavedPiece = null;

            ControlledPiece.transform.position = new Vector3(0, 12, 0);
            ControlledPiece.GetComponent<Pieces>().Placed = false;
            ControlledPiece.SetActive(true);
            StartCoroutine(tempPieces.Descend(0));
        }

        if (tempPieces.Placed)
        {
            int i = Random.Range(0, Pieces.Count);
            ControlledPiece = Instantiate(Pieces[i], Spawner.transform.position, Spawner.transform.rotation);
            tempPieces = ControlledPiece.GetComponent<Pieces>();
        }
    }

    public IEnumerator ReCheckAll(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (GameObject Piece in PlacedBlocks)
        {
            StartCoroutine(Piece.GetComponent<Pieces>().RecheckBlocks(0.2f));
        }
    }
}