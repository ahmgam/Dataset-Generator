using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.UI;
using System.Text;


public class import_module : MonoBehaviour{
	bool model_loaded =false;
	bool folder_loaded=false;
	bool output_loaded=false;
	bool alligned=false;
	bool saved=false;
	int fullAngle=360;
	string outputPath;
	string backgroundsPath;
	float d_angle=30;
	string[] imagesPaths;
	int backgroundIndex=0;
	GameObject loadedModel;
	BoxCollider collider;
	Camera mycam;
	StringBuilder csv = new StringBuilder();
	Light sceneLight ;
	int i =0;


	public void Update()
 	{
		if (model_loaded && folder_loaded && output_loaded &&!alligned)
		{
			if (Input.GetKey(KeyCode.UpArrow))
     		{
        		//do stuff
				loadedModel.transform.position+=new Vector3(0, 0.01f, 0);
     		}
	  		if (Input.GetKey(KeyCode.DownArrow))
     		{
        		//do stuff
				loadedModel.transform.position-=new Vector3(0, 0.01f, 0);
     		}
	  		if (Input.GetKey(KeyCode.RightArrow))
     		{
        		//do stuff
				loadedModel.transform.position+=new Vector3(0.01f, 0, 0);
     		}
	  		if (Input.GetKey(KeyCode.LeftArrow))
     		{
        		//do stuff
				loadedModel.transform.position-=new Vector3(0.01f, 0, 0);
     		}
	  		if (Input.GetKey(KeyCode.P))
     		{
        		//do stuff
				loadedModel.transform.localScale=loadedModel.transform.localScale + new Vector3(0.01f, 0.01f, 0.01f); 
     		}
	  		if (Input.GetKey(KeyCode.M))
     		{
        		//do stuff
				loadedModel.transform.localScale=loadedModel.transform.localScale- new Vector3(0.01f, 0.01f, 0.01f); 
     		}
	  		if (Input.GetKey(KeyCode.Space))
     		{
        		//do stuff
				alligned=true;
				//createCollideBoxes();

				//GenerateData();
     		}
		}
		if (alligned&& !saved)
		{
			GenerateDataFrame ();
		}
	 }
    public void modelImport()
	{

		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Models", ".obj" ) );
		FileBrowser.SetDefaultFilter( ".obj" );
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
		StartCoroutine( ShowLoadDialogCoroutine() );
	}

	IEnumerator ShowLoadDialogCoroutine()
	{

		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Files, false, null, null, "Import Model", "Import" );
		Debug.Log( FileBrowser.Success );
		if( FileBrowser.Success )
		{	
	
            Text myText=GameObject.Find("ModelPath").GetComponent<Text>();
            myText.text=FileBrowser.Result[0];
			model_loaded=true;
	    }
    }

    public void backgroundsImport()
	{

		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Texture", ".mtl" ) );
		FileBrowser.SetDefaultFilter( ".mtl" );
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
		StartCoroutine( ShowLoadDialogCoroutine2() );
	}

	IEnumerator ShowLoadDialogCoroutine2()
	{

		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Folders, false, null, null, "Select Backgrounds Folder", "Select" );
		Debug.Log( FileBrowser.Success );
		if( FileBrowser.Success )
		{
			
            Text myText=GameObject.Find("BackgroundsPath").GetComponent<Text>();
            myText.text=FileBrowser.Result[0];
			folder_loaded=true;
	    }
    }
	    public void outputSelect()
	{

		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Texture", ".mtl" ) );
		FileBrowser.SetDefaultFilter( ".mtl" );
		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
		StartCoroutine( ShowLoadDialogCoroutine3() );
	}

	IEnumerator ShowLoadDialogCoroutine3()
	{

		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.Folders, false, null, null, "Select Output Folder", "Select" );
		Debug.Log( FileBrowser.Success );
		if( FileBrowser.Success )
		{
			
            Text myText=GameObject.Find("OutputPath").GetComponent<Text>();
            myText.text=FileBrowser.Result[0];
			output_loaded=true;
	    }
    }
	public void showModel()
	{
		if (model_loaded && folder_loaded && output_loaded)
		{
		GameObject.Find("model").GetComponent<Button>().enabled=false;
		GameObject.Find("backgrounds").GetComponent<Button>().enabled=false;
		GameObject.Find("show").GetComponent<Button>().enabled=false;
		Canvas mycanv=GameObject.Find("Canvas").GetComponent<Canvas>();
		mycanv.enabled=false;
		mycam=GameObject.Find("MainCamera").GetComponent<Camera>();
		string modelPath= GameObject.Find("ModelPath").GetComponent<Text>().text;
		backgroundsPath = GameObject.Find("BackgroundsPath").GetComponent<Text>().text;
		imagesPaths = Directory.GetFiles(backgroundsPath);
		outputPath= GameObject.Find("OutputPath").GetComponent<Text>().text;
		loadedModel =new OBJLoader().Load(modelPath);
		loadedModel.name="MyModel";
		loadedModel.layer=8;
		//Instantiate(loadedModel);
		mycam.transform.position=new Vector3(Screen.width/2, Screen.height/2, -150); 
		loadedModel.transform.position=new Vector3(Screen.width/2, Screen.height/2, -50); 
		//loadedModel.transform.parent=GameObject.Find("showBoard").transform;
		loadedModel.transform.SetAsLastSibling();
		createCollideBoxes();
		float[] sizeOnScreen=getWandH();
		Debug.Log( sizeOnScreen[0] );
		Debug.Log( sizeOnScreen[1] );
		float desiredScale = 0.3f/(sizeOnScreen[1]/Screen.height);
		loadedModel.transform.localScale =loadedModel.transform.localScale*desiredScale;
		sceneLight = GameObject.Find("Directional Light").GetComponent<Light>();
		sceneLight.transform.LookAt(loadedModel.transform.position);
		Dropdown m_Dropdown=GameObject.Find("Dropdown").GetComponent<Dropdown>();
		d_angle=float.Parse(m_Dropdown.options[m_Dropdown.value].text);

		}
	}
	public void load_background(int i )
	{	
		byte[] fileData = File.ReadAllBytes(imagesPaths[i]);
        Texture2D SpriteTexture = new Texture2D(2, 2);
        SpriteTexture.LoadImage(fileData);
     	Sprite NewSprite  = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), 100.0f);
		Debug.Log( imagesPaths.Length );
		GameObject.Find ("showBackground").GetComponent<Image> ().sprite = NewSprite;

	}

	void createCollideBoxes()
	{
		List<float> Xes = new List<float>();
		List<float> Yes = new List<float>();
		List<float> Zes = new List<float>();
  		Renderer[] ts = loadedModel.GetComponentsInChildren<Renderer>();
		bool hasBounds = false;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		foreach (Renderer t in ts) 
		{
			if (hasBounds)
			{
                bounds.Encapsulate(t.bounds);
        	}
            else 
			{
                bounds = t.bounds;
                hasBounds = true;
            }

		}
		collider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
    	collider.center = bounds.center - loadedModel.transform.position;
    	collider.size = bounds.size;
	}
	float[] getWandH()
	{
		
		Vector3 maxPos = mycam.WorldToScreenPoint(collider.bounds.max);
		Vector3 minPos = mycam.WorldToScreenPoint(collider.bounds.min);
		float[] ret = new float[2];
		ret[0]=(float)(maxPos.x - minPos.x);
		ret[1]=(float)(maxPos.y - minPos.y);
		return ret;
	}
	public void exit_app()
	{
		Application.Quit();
	}
public void GenerateDataFrame ()
	{
		int iter = (int)(fullAngle/d_angle);
        if ((i+1)%(iter*iter*iter)==0 | i==0)
        {
			if (backgroundIndex==imagesPaths.Length)
			{
				Texture2D SpriteTexture= Texture2D.whiteTexture;
				Sprite NewSprite  = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), 100.0f);
				Debug.Log( imagesPaths.Length );
				GameObject.Find ("showBackground").GetComponent<Image> ().sprite = NewSprite;
			}
			else
			{
          		load_background(backgroundIndex);
				backgroundIndex++;
			}
  
        }
		loadedModel.transform.Rotate(d_angle,0.0f,0.0f,Space.Self);
		var randomRotX=Random.Range(-1f,1f);
		var randomRotY=Random.Range(-1f,1f);
		var randomRotZ=Random.Range(-1f,1f);
		var randomIntensity= Random.Range(0.5f,3f);
		Vector3 initialLighting = sceneLight.transform.position;
		sceneLight.transform.position+= new Vector3(randomRotX*1000,randomRotY*1000,randomRotZ*1000);
		sceneLight.intensity= randomIntensity;
		sceneLight.transform.LookAt(loadedModel.transform.position);
		float rot_x=loadedModel.transform.localRotation.eulerAngles.x/fullAngle;
		float rot_y=loadedModel.transform.localRotation.eulerAngles.y/fullAngle;
		float rot_z=loadedModel.transform.localRotation.eulerAngles.z/fullAngle;
		ScreenCapture.CaptureScreenshot(outputPath+"/"+i.ToString()+".png");
		string dataLine = string.Format("{0}.png,{1},{2},{3}", i, rot_x,rot_y,rot_z);
		csv.AppendLine(dataLine);
		sceneLight.transform.position=initialLighting;
		Debug.Log( i );
		i++;
        if ((i+1)%iter==0)
        {
			loadedModel.transform.Rotate(0.0f,d_angle,0.0f,Space.Self);
        }
        if ((i+1)%(iter*iter)==0)
        {
			loadedModel.transform.Rotate(0.0f,0.0f,d_angle,Space.Self);
        }
        if ((i+1)%(iter*iter*iter*(imagesPaths.Length+1))==0)
        {
            File.WriteAllText(outputPath+"/labels.csv", csv.ToString());
            saved=true;
        }
                     		
	}
}
