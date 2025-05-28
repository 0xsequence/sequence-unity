using UnityEditor;
using UnityEngine;

namespace Sequence.SidekickEditor
{
    public class ContractDeployer 
    {
        GUIStyle FoldoutStyle => new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            normal = { textColor = new Color(0.8f, 0.8f, 0.8f) },
            hover = { textColor = Color.white },
            active = { textColor = Color.white }
        };
        
        GUIStyle SectionBoxStyle
        {
            get
            {
                var style = new GUIStyle(GUI.skin.box);
                style.normal.background = TextureMaker.Make(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.7f));
                style.margin = new RectOffset(0, 0, 5, 10);
                style.padding = new RectOffset(10, 10, 10, 10);
                return style;
            }
        }
        
        GUIStyle ButtonStyle
        {
            get
            {
                var style = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 13
                };

                style.normal.textColor = Color.white;
                style.hover.textColor = Color.white;

                return style;
            }
        }

        
        public void SetupFoldout(SidekickEditorController controller, bool deployFoldout, ContractType selectedDeployType)
        {
            deployFoldout = EditorGUILayout.Foldout(deployFoldout, "Deploy", true, FoldoutStyle);
            if (deployFoldout)
            {
                using (new EditorGUILayout.VerticalScope(SectionBoxStyle))
                {
                    selectedDeployType = (ContractType)EditorGUILayout.EnumPopup("Contract Type", selectedDeployType);

                    EditorGUILayout.Space();

                    switch (selectedDeployType)
                    {
                        case ContractType.CurrencyToken:
                            controller.DeployERC20InitialOwner = EditorGUILayout.TextField("Initial Owner", controller.DeployERC20InitialOwner);
                            controller.DeployERC20Name = EditorGUILayout.TextField("Name", controller.DeployERC20Name);
                            controller.DeployERC20Symbol = EditorGUILayout.TextField("Symbol", controller.DeployERC20Symbol);
                            GUILayout.Space(8);
                            if (GUILayout.Button("Deploy Currency Token", ButtonStyle, GUILayout.Height(26)))
                                controller.DeployERC20();
                            break;

                        case ContractType.NFTContract:
                            controller.DeployERC721DefaultAdmin = EditorGUILayout.TextField("Default Admin", controller.DeployERC721DefaultAdmin);
                            controller.DeployERC721Minter = EditorGUILayout.TextField("Minter", controller.DeployERC721Minter);
                            controller.DeployERC721Name = EditorGUILayout.TextField("Name", controller.DeployERC721Name);
                            controller.DeployERC721Symbol = EditorGUILayout.TextField("Symbol", controller.DeployERC721Symbol);
                            GUILayout.Space(8);
                            if (GUILayout.Button("Deploy NFT Contract", ButtonStyle, GUILayout.Height(26)))
                                controller.DeployERC721();
                            break;

                        case ContractType.Web3GameItem:
                            controller.DeployDefaultAdmin = EditorGUILayout.TextField("Default Admin", controller.DeployDefaultAdmin);
                            controller.DeployMinter = EditorGUILayout.TextField("Minter", controller.DeployMinter);
                            controller.DeployName = EditorGUILayout.TextField("Name", controller.DeployName);
                            GUILayout.Space(8);
                            if (GUILayout.Button("Deploy Web 3 Game Item", ButtonStyle, GUILayout.Height(26)))
                                controller.DeployERC1155();
                            break;
                    }
                }
            }
        }
    }
}