using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


// Esta función permite cargar un AudioClip a partir de la dirección de un archivo de audio

public class LoadAudioFiles : MonoBehaviour
{
    WWW www; // Objeto que permite la lectura de archivos a partir de su dirección
    public AudioClip chosenClip; // AudioClip del archivo seleccionado

    PlayClip player; // Clase PlayClip implementada en el GameObject AudioPlayer

    Text canvasText; // Texto superior de la interfaz gráfica

    void Start()
    {
        // Búsqueda del GameObject canvasText implementado en la interfaz gráfica
        canvasText = GameObject.Find("Texto").GetComponent<Text>();
        // Búsqueda de la clase PlayClip implementada en el GameObject AudioPlayer
        player = GameObject.Find("AudioPlayer").GetComponent<PlayClip>();
    }

    // Función para cargar el archivo seleccionado a partir de su dirección
    public void LoadClip(string chosenFilePath)
    {
        StartCoroutine(LoadAudio(chosenFilePath));
    }

    /* 
        Función adicional para cargar el archivo apropiadamente. Al ser un archivo de audio WAV,
        es necesario esperar a que esté cargado por completo para proceder con su procesamiento y
        reproducción. Esto es posible mediante las funciones de tipo IEnumerator.
     */
    IEnumerator LoadAudio(string chosenFilePath)
    {
        www = new WWW(chosenFilePath); // Obtención del archivo WAV
        yield return www;
        chosenClip = www.GetAudioClip(false, false); // Obtención del AudioClip a partir del archivo seleccionado
        player.loadedClip = chosenClip;  // Asignación del AudioClip cargado a la clase PlayClip
        www = null;
        player.AudioLoaded(); // Activación de la función AudioLoaded() de la clase PlayClip
        chosenClip = null;
    }
}
