using Amazon;
using Amazon.CognitoIdentity;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetPhotos : MonoBehaviour
{

    // Use this for initialization
    private string S3BucketName = "photouploaderforet";
    public GameObject imagePrefab;
    public GameObject scrollContent;

    public static Sprite sprite;
    public static string photoName;

    void Start()
    {
        displayPhotos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static Sprite getSprite()
    {
        return sprite;
    }

    public static string getName()
    {
        return photoName;
    }

    private void displayPhotos()
    {
        foreach (Transform n in scrollContent.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/photos");
        FileInfo[] info = dir.GetFiles("*.jpg");
        Sprite[] image = Resources.LoadAll<Sprite>("photos/");
        foreach (Sprite s in image)
        {
            GameObject imageobj = Instantiate(imagePrefab, scrollContent.transform);
            imageobj.transform.Find("Image").gameObject.GetComponent<Image>().sprite = s;
            imageobj.transform.Find("Text").gameObject.GetComponent<Text>().text = s.name;
        }
    }

    public void download()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "us-east-1:931d3d0b-6093-4852-b65c-9676aa85e17c", // ID プールの ID
            RegionEndpoint.USEast1 // リージョン
        );
        AmazonS3Client S3Client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
        // ResultText is a label used for displaying status information
        print("Fetching all the Objects from " + S3BucketName);

        var request = new ListObjectsRequest()
        {
            BucketName = S3BucketName
        };

        S3Client.ListObjectsAsync(request, (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                print("Got Response \nPrinting now \n");
                responseObject.Response.S3Objects.ForEach((o) =>
                {
                    print(o.Key);
                    S3Client.GetObjectAsync("photouploaderforet", o.Key, (res) =>
                    {
                        string data = null;
                        var response = res.Response;
                        if (response.ResponseStream != null)
                        {
                            UnityInitializer.AttachToGameObject(this.gameObject);
                            print(Application.dataPath + "/" + o.Key);
                            if (!Directory.Exists(Application.dataPath + "/Resources/photos"))
                            {
                                Directory.CreateDirectory(Application.dataPath + "/Resources/photos");
                            }
                            using (var fs = System.IO.File.Create(Application.dataPath + "/Resources/" + o.Key))
                            {
                                byte[] buffer = new byte[81920];
                                int count;
                                while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
                                    fs.Write(buffer, 0, count);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            print("error");
                        }
                        displayPhotos();
                    });
                });
            }
            else
            {
                print("Got Exception \n");
                print(responseObject.Exception.ToString());
            }
        });
    }

    public void setTargetPhoto(Sprite spr)
    {
        sprite = spr;
        SceneManager.LoadScene("SelectArea");
    }

    public void setPhotoName(string name)
    {
        photoName = name;
    }
}
