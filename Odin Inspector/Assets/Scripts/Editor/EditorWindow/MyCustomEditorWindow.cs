using Sirenix.OdinInspector;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

public class CustomEditorWindow : OdinEditorWindow
{
    private PropertyTree propertyTree;

    [OnInspectorGUI("DrawPreview",append:true)]
    public int index;
    

    [MenuItem("自定义窗口/Custom Editor")]
    private static void OpenWindow()
    {
        GetWindow<CustomEditorWindow>().Show();
    }
    
    private void DrawPreview()
    {
        if(index <0) return;

        TestData testData = new TestData();
        PropertyTree tree = PropertyTree.Create(testData);
        tree.Draw(false);

    }
    
}
