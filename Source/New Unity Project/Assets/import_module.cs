using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.UI;
using System.Text;
using System.Threading;

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
	List<string> DataLine =new List<string>() {"","","",""};

	GameObject loadedModel;
	BoxCollider collider;
	Camera mycam;
	StringBuilder csv = new StringBuilder();
	Light sceneLight ;
	Vector3 initialLighting;
	Vector3 initialPos;
	
	List<Sprite> LoadedBackgrounds =new List<Sprite>() ;
	int i =0;

	public void Start()
	{
		 Screen.SetResolution(416, 416, false);



	}

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
			if (Input.GetKey(KeyCode.K))
     		{

	
				
				createCollideBoxes();

	
     		}
		}
		if (alligned&& !saved)
		{
			WriteLabels ();
			GenerateDataFrame2 ();
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
		LoadBackgrounds();
		outputPath= GameObject.Find("OutputPath").GetComponent<Text>().text;
		loadedModel =new OBJLoader().Load(modelPath);
		collider = loadedModel.AddComponent<BoxCollider>() as BoxCollider;
		loadedModel.name="MyModel";
		loadedModel.layer=8;
		//Instantiate(loadedModel);
		mycam.transform.position=new Vector3(500, 800, 0); 
		loadedModel.transform.position=new Vector3(500, 800, 100); 
		//loadedModel.transform.parent=GameObject.Find("showBoard").transform;
		loadedModel.transform.SetAsLastSibling();
		createCollideBoxes();
		float[] sizeOnScreen=getWandH();
		float desiredScale = 0.3f/(sizeOnScreen[1]/Screen.height);
		loadedModel.transform.localScale =loadedModel.transform.localScale*desiredScale;
		sceneLight = GameObject.Find("Directional Light").GetComponent<Light>();
		sceneLight.transform.LookAt(loadedModel.transform.position);
		createCollideBoxes();
		
		}
	}
	public void selectionChanged()
	{
		TMPro.TMP_Dropdown m_Dropdown=GameObject.Find("AngleResolutionSelect").GetComponent<TMPro.TMP_Dropdown>();
		d_angle=float.Parse(m_Dropdown.options[m_Dropdown.value].text);
	}

	void recalculateBounds()
	{
		MeshFilter[] ts = loadedModel.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter t in ts) 
		{
			t.mesh.RecalculateBounds();
		}
	}
	void createCollideBoxes()
	{
		//recalculateBounds();
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

    	collider.center = bounds.center - loadedModel.transform.position;
    	collider.size = bounds.size;
	}
	void LoadBackgrounds()
	{
		foreach (string path in imagesPaths)
		{
			
        Texture2D SpriteTexture = new Texture2D(2, 2);
        SpriteTexture.LoadImage(File.ReadAllBytes(path));
     	Sprite NewSprite  = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), 100.0f);
			LoadedBackgrounds.Add(NewSprite );
		}
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

	public int[] getWH (string path="")
	{
		Texture2D img=new Texture2D(2,2);
		if (path!="")
		{
			byte[] fileData = File.ReadAllBytes(path);
			img.LoadImage(fileData);
		}
		else
		{
			img = ScreenCapture.CaptureScreenshotAsTexture();
		}
		int[] wh= new int[4];
		int minxIndex=img.width;
		int maxxIndex=0;
		int minyIndex =img.height;
		int maxyIndex =0;

		for (int i = 0; i < img.width; i++)
		{
    		for (int j = 0; j < img.height; j++)
    		{
        		Color pixel = img.GetPixel(i,j);
				if ((pixel.r!=1)|(pixel.g!=1)|(pixel.b!=1))
				{
					if (j<minyIndex)
					{
						minyIndex=j;
						break;
					}
				
        		
    			}
			}
		} 
		for (int i = 0; i < img.width; i++)
		{
    		for (int j = img.height; j > 0; j--)
    		{
        		Color pixel = img.GetPixel(i,j);
				if ((pixel.r!=1)|(pixel.g!=1)|(pixel.b!=1))
				{
					if (j>maxyIndex)
					{
						maxyIndex=j;
						break;
					}
				}
				
        		
    		}
		} 
		for (int i = 0; i < img.height; i++)
		{
    		for (int j = 0; j < img.width; j++)
    		{
        		Color pixel = img.GetPixel(j,i);
				if ((pixel.r!=1)|(pixel.g!=1)|(pixel.b!=1))
				{
					if (j<minxIndex)
					{
						minxIndex=j;
						break;
					}
				}
				
        		
    		}
		} 
		for (int i = 0; i <img.height; i++)
		{
    		for (int j = img.width; j > 0; j--)
    		{
        		Color pixel = img.GetPixel(j,i);
				if ((pixel.r!=1)|(pixel.g!=1)|(pixel.b!=1))
				{
					if (j>maxxIndex)
					{
						maxxIndex=j;
						break;
					}
				
				}
				
        		
    		}
		} 
		wh[0]=minxIndex;
		wh[1]=maxxIndex;
		wh[2]=minyIndex;
		wh[3]=maxyIndex;
		GameObject.Destroy(img);
		return wh;
	}
	
public void GenerateDataFrame2 ()
	{
		if (saved==false)
		{

		
		Debug.Log( i );
		int iter = (int)(fullAngle/d_angle);
		if ((i%(imagesPaths.Length+1))==imagesPaths.Length)
		{
			GameObject.Find ("showBackground").GetComponent<Image> ().sprite = LoadedBackgrounds[imagesPaths.Length-1];
			ScreenCapture.CaptureScreenshot(outputPath+"/"+i.ToString("D6").PadLeft(6, '0')+".png");
			DataLine[0]=i.ToString("D6").PadLeft(6, '0')+".png";
			//File.AppendAllText(outputPath+"/labels.csv",i.ToString("D6")+".png," +DataLine);
			i++;
			sceneLight.transform.position=initialLighting;
			loadedModel.transform.position=initialPos;
		}
		else if ((i%(imagesPaths.Length+1))==0)
		{
			
				   Texture2D whitetext=Texture2D.whiteTexture;
	Sprite whtite;
		whtite=Sprite.Create(whitetext, new Rect(0, 0, whitetext.width, whitetext.height),new Vector2(0,0), 100.0f);
			
			GameObject.Find ("showBackground").GetComponent<Image> ().sprite =whtite;
			loadedModel.transform.Rotate(d_angle,0.0f,0.0f,Space.Self);
			//createCollideBoxes();
			var randomRotX=Random.Range(-1f,1f);
			var randomRotY=Random.Range(-1f,1f);
			var randomRotZ=Random.Range(-1f,1f);
			var randomXtransform =(mycam.ScreenToWorldPoint(new Vector3(mycam.pixelWidth ,loadedModel.transform.position.y, transform.position.z)).x - loadedModel.transform.position.x) *Random.Range(-0.75f,0.75f);
			var randomYtransform =(mycam.ScreenToWorldPoint(new Vector3(loadedModel.transform.position.x ,mycam.pixelHeight, transform.position.z)).y - loadedModel.transform.position.y) *Random.Range(-0.6f,0.6f);
			var randomIntensity= Random.Range(0.5f,2f);
			initialLighting = sceneLight.transform.position;
			sceneLight.transform.position+= new Vector3(randomRotX*1000,randomRotY*1000,randomRotZ*1000);
			sceneLight.intensity= randomIntensity;
			sceneLight.transform.LookAt(loadedModel.transform.position);
	
			float rot_x=loadedModel.transform.localRotation.eulerAngles.x;
			float rot_y=loadedModel.transform.localRotation.eulerAngles.y;
			float rot_z=loadedModel.transform.localRotation.eulerAngles.z;
			initialPos=loadedModel.transform.position;
			loadedModel.transform.position+=new Vector3(randomXtransform,randomYtransform,0);
			ScreenCapture.CaptureScreenshot(outputPath+"/"+i.ToString("D6").PadLeft(6, '0')+".png");
			//int[] WHarray = getWH (outputPath+"/"+i.ToString("D6").PadLeft(6, '0')+".png");
			//int w = WHarray[1]-WHarray[0];
			//int h = WHarray[3]-WHarray[2];
			DataLine[0]=i.ToString("D6").PadLeft(6, '0')+".png" ;
			DataLine[1]= Mathf.Abs(rot_x).ToString("N5");
			DataLine[2]=Mathf.Abs(rot_y).ToString("N5");
			DataLine[3]=Mathf.Abs(rot_z).ToString("N5");
			//File.AppendAllText(outputPath+"/labels.csv",i.ToString("D6")+".png," +DataLine);
			i++;
		}
		else 
		{
			GameObject.Find ("showBackground").GetComponent<Image> ().sprite = LoadedBackgrounds[(i%(imagesPaths.Length+1))-1];
			ScreenCapture.CaptureScreenshot(outputPath+"/"+i.ToString("D6").PadLeft(6, '0')+".png");
			DataLine[0]=i.ToString("D6").PadLeft(6, '0')+".png";
			//File.AppendAllText(outputPath+"/labels.csv",i.ToString("D6")+".png," +DataLine);
			i++;
		}
				
        if ((i+1)%(iter*(imagesPaths.Length+1))==0)
        {
			loadedModel.transform.Rotate(0.0f,d_angle,0.0f,Space.Self);
        }
        if ((i+1)%(iter*iter*(imagesPaths.Length+1))==0)
        {
			loadedModel.transform.Rotate(0.0f,0.0f,d_angle,Space.Self);
        }
        if ((i+1)%(iter*iter*iter*(imagesPaths.Length+1))==0)
        {
            //File.WriteAllText(outputPath+"/labels.csv", csv.ToString());
            saved=true;
        }
		}
		else
		{
			exit_app();
		}         		
	}

	void WriteLabels ()
	{
		if (DataLine[0]!="") 
		{
			int[] WHarray = getWH (outputPath+"/"+DataLine[0]);
			int w = WHarray[1]-WHarray[0];
			int h = WHarray[3]-WHarray[2];
			int x = WHarray[0]+(int)w/2;
			int y = WHarray[2]+(int)h/2;
			File.AppendAllText(outputPath+"/labels.csv",string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\n", DataLine[0],x,y,w,h,DataLine[1],DataLine[2],DataLine[3]));
		}
	}
}


