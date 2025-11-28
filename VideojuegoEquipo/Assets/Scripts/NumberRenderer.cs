using UnityEngine;
using UnityEngine.UI;

public class NumberRenderer : MonoBehaviour
{
    [Header("Recursos")]
    // Arrastra aquí tus sprites del 0 al 9 en orden
    public Sprite[] numberSprites;

    [Header("UI Images")]
    // Arrastra aquí las imágenes de tu Canvas que mostrarán los números
    // Para el puntaje: Arrastra las imágenes de izquierda a derecha (Centenas, Decenas, Unidades)
    public Image[] digitImages;

    public void UpdateNumber(int value)
    {
        // Convertimos el número a string para procesar dígito por dígito
        // "D" + digitImages.Length asegura que si el número es 5 y hay 3 espacios, escriba "005"
        string scoreString = value.ToString("D" + digitImages.Length);

        for (int i = 0; i < digitImages.Length; i++)
        {
            // Obtenemos el carácter numérico (ej. '5')
            char digitChar = scoreString[i];

            // Lo convertimos a int (ej. 5)
            int spriteIndex = int.Parse(digitChar.ToString());

            // Asignamos el sprite correspondiente
            if (spriteIndex < numberSprites.Length)
            {
                digitImages[i].sprite = numberSprites[spriteIndex];
            }
        }
    }
}