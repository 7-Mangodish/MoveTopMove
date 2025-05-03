using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour {
    [SerializeField] Image whereToShowScreenShot;
    private string folderPath;

    private void Start() {
        folderPath = Path.Combine(Application.dataPath, "Images");
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
    }

    private IEnumerator TakeScreenShotAndShow() {
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D newScreenShot = new Texture2D(screenshot.width, screenshot.height, TextureFormat.RGB24, false);
        newScreenShot.SetPixels(screenshot.GetPixels());
        newScreenShot.Apply();
        Destroy(screenshot);
        Sprite screenshotSprite = Sprite.Create(newScreenShot, new Rect(0, 0, newScreenShot.width, newScreenShot.height), new Vector2(0.5f, 0.5f));
        whereToShowScreenShot.enabled = true;
        whereToShowScreenShot.sprite = screenshotSprite;
        byte[] bytes = newScreenShot.EncodeToPNG();
        string filePath = Path.Combine(folderPath, "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Screenshot saved to: " + filePath);
    }
}