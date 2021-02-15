using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    La clase ButtonClicked contiene las funciones de los diferentes botones de la
    interfaz gráfica
     */

public class ButtonClicked : MonoBehaviour
{
    PlayClip player; // Clase PlayClip implementada en la aplicación
    public GameObject pauseButton, playButton; // GameObjects de pausa y reproducción

    void Start()
    {
        // Búsqueda de la clase PlayClip implementada en el GameObject AudioPlayer
        player = GameObject.Find("AudioPlayer").GetComponent<PlayClip>();
        // Búsqueda del GameObject de reproducción
        playButton = GameObject.Find("PlayButton");
        // Búsqueda del GameObject de pausa
        pauseButton = GameObject.Find("PauseButton");
    }

    // Función que permite seleccionar el método de reproducción original
    public void OriginalButtonClicked()
    {
        player.ChangeMethod("original");
    }

    // Función que permite seleccionar el método de reproducción Nearfield Crosstalk
    public void ManorButtonClicked()
    {
        player.ChangeMethod("manor");
    }
    // Función que permite seleccionar el método de reproducción eaGoggles
    public void GilchrestButtonClicked()
    {
        player.ChangeMethod("gilchrest");
    }

    // Función para pausar el audio
    public void PauseButtonClicked()
    {
        player.Pause();
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }

    // Función para reproducir el audio
    public void PlayButtonClicked()
    {
        player.Play();
        pauseButton.SetActive(true);
        playButton.SetActive(false);
    }
}
