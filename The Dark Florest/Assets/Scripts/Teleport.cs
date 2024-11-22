using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Adicionado para o carregamento de cenas
using System.Collections;

public class TeleportPlayerWithFade : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Nome da cena a ser carregada
    [SerializeField] private Image fadeImage; // Imagem preta para o efeito de fade
    [SerializeField] private float fadeDuration = 1f; // Duração do fade em segundos
    [SerializeField] private PuzzleManager puzzleManager; // Referência ao PuzzleManager

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Verifica se a porta está aberta antes de carregar a cena
            if (puzzleManager != null && puzzleManager.isDoorOpen)
            {
                StartCoroutine(LoadSceneWithFade());
            }
            else
            {
                Debug.Log("A porta está fechada! Resolva o puzzle para abrir.");
            }
        }
    }

    private IEnumerator LoadSceneWithFade()
    {
        // Ativa o fade para preto
        yield return StartCoroutine(FadeToBlack());

        // Carrega a cena
        SceneManager.LoadScene(sceneToLoad);

        // Espera a cena carregar (o fade já foi feito, então esse passo pode ser opcional)
        yield return null;

        // Retorna o fade para transparente (não necessário, pois a nova cena já foi carregada)
        // yield return StartCoroutine(FadeToClear());
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    // O FadeToClear pode ser desnecessário, pois a cena já estará carregada quando o fade acabar.
}