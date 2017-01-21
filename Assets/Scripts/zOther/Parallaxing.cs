using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour
{

    public Transform[] backgrounds;     // tablica z Fore/Backgrounds
    private float[] parallaxScales;     // poruszanie się teł
    public float smoothing = 1;         // gładkość poruszania, musi być większe od 0

    private Transform cam;
    private Vector3 previousCamPos;       // pozycja kamery w poprzedniej klatce

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void Start()
    {
        // pozycja kamery w poprzedniej klatce = poz kam w aktualnej klatce
        previousCamPos = cam.position; // cam.position pozycja kamery z tej klatki

        parallaxScales = new float[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;

        }
    }

    void Update()
    {
        // dla każdego tła
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // parallax jest odwrotny do ruchu kamery ponieważ poprzednia klatka jest pomnożona przez parallaxScales
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

            // do parallaxY
            // float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i] / 3;

            // ustawienie pozycji x celu która wynosi aktualna pozycja + parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            // do parallaxY
            // ustawienie pozycji y celu która wynosi aktualna pozycja + parallax
            // float backgroundTargetPosY = backgrounds[i].position.y + parallaxY;

            // stwórz pozycje celu która jest aktualną pozycją tła z pozycją x celu
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
            // ^ do parallaxY dopisać: backgroundTargetPosX

            // fade pomiędzy aktualną pozycją a pozycją docelową używając lerp (derp?)
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        // ustaw PreviousCamPos na pozycję kamery pod koniec klatki
        previousCamPos = cam.position;
    }
}
