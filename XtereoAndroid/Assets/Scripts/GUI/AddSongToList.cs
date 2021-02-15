using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/*
 La clase AddSongToList está asignada al GameObject GUI/Canvas.

 Esta clase tiene dos funciones principales:
    
    - GetFilesInDirectory(): Selecciona el directorio en el cual se encontrarán
    guardados los archivos WAV que reproducirá la aplicación. Debido a limitantes
    de Unity, fue necesario arrastrar manualmente los archivos de audio dentro de
    la memoria del dispositivo móvil a una carpeta 'WavFiles' dentro del directorio
    de la aplicación. La variable fileInfo almacena las direcciones de cada uno de
    los archivos WAV encontrados en la carpeta.

    - ShowFilesInScrollView(): Por cada archivo WAV encontrado, esta función crea un
    botón dentro del ScrollView de la interfaz gráfica. Se crea una copia de un botón
    'template', el cual se encuentra por fuera del rango visible de la aplicación. Cada
    copia tiene como función 'onClick' la activación de la función playAudio() de la 
    clase PlayClip, la cual recibe la dirección y el nombre del archivo a reproducir.

    Las dos funciones anteriormente mencionadas se ejecutan en la función start().

    La aplicación está restringida a la reproducción de archivos WAV estereofónicos a 44.1 kHz de muestreo. 
  
*/

public class AddSongToList : MonoBehaviour
{
    FileInfo[] fileinfo; // Arreglo de direcciones de archivos WAV
    
    public GameObject template; // Botón para generar copias por cada archivo WAV
    public GameObject content; // Contenedor de los botones dentro del ScrollView

    void Start()
    {
        GetFilesInDirectory();
        ShowFilesInScrollView();
    }

    void GetFilesInDirectory()  // Búsqueda de archivos WAV dentro del directorio
    {
        string path = "";
        // Selección de directorio según la plataforma de ejecución
        switch (Application.platform) 
        {
            case RuntimePlatform.WindowsEditor: // Unity en Windows
                path = "C:/Users/mateo/Music/WavFiles";
                break;
            case RuntimePlatform.Android: // Aplicación de Android
                path = Application.persistentDataPath + "/WavFiles";
                break;

        }
        DirectoryInfo dataDir = new DirectoryInfo(path);
        // Búsqueda de archivos en formato WAV dentro del directorio seleccionado
        fileinfo = dataDir.GetFiles("*.wav", SearchOption.AllDirectories); 
    }

    void ShowFilesInScrollView()  // Creación de botones por cada archivo WAV encontrado
    {
        // El contador i itera entre el número de archivos WAV encontrados
        for (int i = 0; i < fileinfo.Length; i++)
        {
            var copy = Instantiate(template); // Cppia del botón 'template'
            copy.transform.SetParent(content.transform); // Ajuste del tamaño del botón
            // Asignación del nombre del archivo WAV al texto que muestra cada botón
            Text buttonText = copy.GetComponentInChildren<Text>();
            string filePath = fileinfo[i].ToString();
            buttonText.text = GetFileName(filePath);

            /*  Es necesario crear una copia del contador para asignarlo a la función
                onClick() de cada botón
             */
            int copyOfI = i;
            string fileName = buttonText.text;

            /*  Este código permite añadir una función para cada botón al ser ejecutado
                Cada botón permite reproducir uno de los archivos WAV encontrados.             
             */
            copy.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    // Búsqueda del GameObject AudioPlayer dentro de la aplicación
                    GameObject playObject = GameObject.Find("AudioPlayer");
                    // Búsqueda de la clase PlayClip implementada en el objeto AudioPlayer
                    PlayClip player = GameObject.Find("AudioPlayer").GetComponent<PlayClip>();
                    /* Dirección del archivo seleccionado. El contador copyOfI permite
                        seleccionar la dirección del archivo de audio al cual se creó el botón
                     */                
                    string chosenFilePath = "file://" + fileinfo[copyOfI].FullName;
                    // Activación de la función PlayAudio() de la clase PlayClip
                    player.PlayAudio(chosenFilePath, fileName);
                }
                );
        }
    }

    string GetFileName(string filePath) // Obtención del nombre del archivo desde su directorio
    {
        int numChars = filePath.Length;
        int cont = numChars;
        bool found = false;
        char c = ' ';
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                c = '\\';
                break;
            case RuntimePlatform.Android:
                c = '/';
                break;
        }
        while (!found)
        {
            cont--;
            if (filePath[cont] == c)
            {
                found = true;
            }
        }
        string newName = filePath.Remove(0, cont + 1);
        newName = newName.Remove(newName.Length - 4, 4);
        return newName;
    }
}
