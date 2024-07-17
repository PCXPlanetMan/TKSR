using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

public class test : MonoBehaviour
{
    public Image Pic;
    
    // Start is called before the first frame update
    // private IEnumerator Start()
    // {
    //     var initializeAsync = Assets.InitializeAsync();
    //     yield return initializeAsync;
    // }
    
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    IEnumerator Start()
    {
       
        yield return InitializeYooAsset();
    }
    
    private IEnumerator InitializeYooAsset()
    {
        // 初始化资源系统
        YooAssets.Initialize();
        
        // 创建默认的资源包
        var package = YooAssets.CreatePackage("TKSRDefaultPackage");

        // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
        YooAssets.SetDefaultPackage(package);
        
        var initParameters = new EditorSimulateModeParameters();
        var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "TKSRDefaultPackage");
        initParameters.SimulateManifestFilePath  = simulateManifestFilePath;
        yield return package.InitializeAsync(initParameters);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickTest()
    {
        LoadAsync();
    }
    
    public void LoadAsync()
    {
        StartCoroutine(_Loading());
    }

    private IEnumerator _Loading()
    {
        var package = YooAssets.GetPackage("TKSRDefaultPackage");
        _spriteHandle = package.LoadAssetAsync<Sprite>("Topaz-Fa_1-1");
        yield return _spriteHandle;
        
        SetSprite(_spriteHandle);
    }
    
    private AssetHandle _spriteHandle;
    

    public void Unload()
    {
        if (_spriteHandle != null)
        {
            _spriteHandle.Release();
            _spriteHandle = null;
        }
    }
    
    private void SetSprite(AssetHandle asset)
    {
        Pic.sprite = asset.AssetObject as Sprite;
        Pic.SetNativeSize();
    }
}
