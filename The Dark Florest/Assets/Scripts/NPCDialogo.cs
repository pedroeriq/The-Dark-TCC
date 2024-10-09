using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogo : MonoBehaviour
{
    public string[] dialogoNPC;
    public int dialogoIndex;

    public GameObject dialogoPainel;
    public Text dialogoText;

    public Text nameNPC;    
    public Image imageNPC;
    public Sprite spriteNPC;

    public bool readToSpeak;
    public bool startDialogo;

    // Start is called before the first frame update
    void Start()
    {
        dialogoPainel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && readToSpeak)
        {
            if (!startDialogo)
            {
                FindObjectOfType<Player>().speed = 0f;
                StartDialogo();
            }
            else if (dialogoText.text == dialogoNPC[dialogoIndex])
            {
                ProximoDialogo();
            }
        }
    }

    void ProximoDialogo()
    {
        dialogoIndex++;

        if (dialogoIndex < dialogoNPC.Length)
        {
            StartCoroutine(MostrarDialogo());
        }
        else
        {
            dialogoPainel.SetActive(false);
            startDialogo = false;
            dialogoIndex = 0;
            FindObjectOfType<Player>().speed = 7f;
        }
    }

    void StartDialogo()
    {
        nameNPC.text = "Yan";
        imageNPC.sprite = spriteNPC;
        startDialogo = true;
        dialogoIndex = 0;
        dialogoPainel.SetActive(true);
        StartCoroutine(MostrarDialogo());
    }

    IEnumerator MostrarDialogo()
    {
        dialogoText.text = "";
        foreach (char letter in dialogoNPC[dialogoIndex])
        {
            dialogoText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            readToSpeak = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            readToSpeak = false;
        }
    }
}