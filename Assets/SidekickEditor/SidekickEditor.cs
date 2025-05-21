using UnityEditor;
using UnityEngine;
using Sequence.Sidekick;

[CustomEditor(typeof(SidekickEditorController))]
public class SidekickEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
        SidekickEditorController controller = (SidekickEditorController)target;

        #region Deploy ERC1155

        EditorGUILayout.LabelField("Deploy ERC1155");

        controller.deployDefaultAdmin = EditorGUILayout.TextField("Default Admin", controller.deployDefaultAdmin);
        controller.deployMinter = EditorGUILayout.TextField("Minter", controller.deployMinter);
        controller.deployName = EditorGUILayout.TextField("Name", controller.deployName);

        if (GUILayout.Button("Deploy ERC1155"))
        {
            controller.DeployERC1155(controller.deployDefaultAdmin, controller.deployMinter, controller.deployName);
        }

    #endregion

    #region Mint ERC1155

        EditorGUILayout.LabelField("Mint ERC1155");
        controller.MintContractAddress = EditorGUILayout.TextField("Contract Address", controller.MintContractAddress);
        controller.MintRecipientAddress = EditorGUILayout.TextField("Recipient Address", controller.MintRecipientAddress);
        controller.MintId = EditorGUILayout.TextField("ID", controller.MintId);
        controller.MintAmount = EditorGUILayout.TextField("Amount", controller.MintAmount);
        controller.MintData = EditorGUILayout.TextField("Data", controller.MintData);

        if (GUILayout.Button("Mint ERC1155"))
        {
            controller.MintERC1155(controller.MintContractAddress, controller.MintRecipientAddress, controller.MintId, controller.MintAmount, controller.MintData);
        }

    #endregion

        base.OnInspectorGUI();
    }
}