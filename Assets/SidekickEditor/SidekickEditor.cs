using UnityEditor;
using UnityEngine;
using System;

namespace Sequence.SidekickEditor
{
    [CustomEditor(typeof(SidekickEditorController))]
    public class SidekickEditor : UnityEditor.Editor
    {
        bool deployFoldout = true;
        bool mintFoldout = true;
        bool generateFoldout = true;
        bool grantRoleFoldout = true;

        ContractType selectedDeployType;
        ContractType selectedMintType;
        ContractType selectedGenerateType;

        GUIStyle HeaderStyle => new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
        };

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
                style.normal.background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.7f));
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

        GUIStyle LabelBoldStyle => new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            normal = { textColor = Color.white }
        };

        public override void OnInspectorGUI()
        {
            var controller = (SidekickEditorController)target;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Sidekick Wallet", HeaderStyle);

            GUILayout.Space(6);
            if (GUILayout.Button("Get Wallet Address", ButtonStyle, GUILayout.Height(28)))
                controller.GetWalletAddress();

            //GUILayout.Space(6);
            //if (GUILayout.Button("Get Contracts", ButtonStyle, GUILayout.Height(28)))
            //    controller.GetContracts();

            EditorGUILayout.Space(14);

            new ContractDeployer().SetupFoldout(controller, deployFoldout, selectedDeployType);

            #region Grant Role

            grantRoleFoldout = EditorGUILayout.Foldout(grantRoleFoldout, "Grant Role", true, FoldoutStyle);
            if (grantRoleFoldout)
            {
                using (new EditorGUILayout.VerticalScope(SectionBoxStyle))
                {
                    controller.SelectedContractRole =
                        (ContractRole)EditorGUILayout.EnumPopup("Role", controller.SelectedContractRole);

                    switch (controller.SelectedContractRole)
                    {
                        case ContractRole.Minter:
                            controller.GrantRoleRole =
                                "0xdfd8f10cabf255ee49ecf3db8b4502eea7eac26c270f6e5f0f28506f6f3fbb55";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(controller.SelectedContractRole),
                                "Unsupported role");
                    }

                    controller.GrantRoleContractAddress =
                        EditorGUILayout.TextField("Contract Address", controller.GrantRoleContractAddress);
                    controller.GrantRoleAccount =
                        EditorGUILayout.TextField("Account Address", controller.GrantRoleAccount);

                    GUILayout.Space(8);

                    if (GUILayout.Button("Grant Role", ButtonStyle, GUILayout.Height(26)))
                        controller.GrantERC1155Role();
                }
            }

            #endregion

            #region Mint

            mintFoldout = EditorGUILayout.Foldout(mintFoldout, "Mint", true, FoldoutStyle);
            if (mintFoldout)
            {
                using (new EditorGUILayout.VerticalScope(SectionBoxStyle))
                {
                    selectedMintType = (ContractType)EditorGUILayout.EnumPopup("Item Type", selectedMintType);
                    GUILayout.Space(6);

                    switch (selectedMintType)
                    {
                        case ContractType.CurrencyToken:
                            controller.MintERC20ContractAddress = EditorGUILayout.TextField("Currency Address",
                                controller.MintERC20ContractAddress);
                            controller.MintERC20RecipientAddress = EditorGUILayout.TextField("Recipient Address",
                                controller.MintERC20RecipientAddress);
                            controller.MintERC20Amount =
                                EditorGUILayout.TextField("Amount", controller.MintERC20Amount);
                            GUILayout.Space(8);
                            if (GUILayout.Button("Mint CurrencyToken", ButtonStyle, GUILayout.Height(26)))
                                controller.MintERC20();
                            break;

                        case ContractType.NFTContract:
                            controller.SafeMintERC721ContractAddress = EditorGUILayout.TextField("NFT Contract Address",
                                controller.SafeMintERC721ContractAddress);
                            controller.IsBatchMintERC721 =
                                EditorGUILayout.Toggle("Batch Mint", controller.IsBatchMintERC721);

                            GUILayout.Space(8);

                            if (controller.IsBatchMintERC721)
                            {
                                int batchSize = controller.SafeMintERC721BatchRecipients?.Length ?? 0;
                                batchSize = EditorGUILayout.IntField("Batch Size", batchSize);
                                batchSize = Mathf.Max(batchSize, 0);
                                GUILayout.Space(6);

                                var recipients = controller.SafeMintERC721BatchRecipients ?? new string[0];
                                var tokenIds = controller.SafeMintERC721BatchTokenIds ?? new string[0];

                                if (recipients.Length != batchSize)
                                    Array.Resize(ref recipients, batchSize);
                                if (tokenIds.Length != batchSize)
                                    Array.Resize(ref tokenIds, batchSize);

                                for (int i = 0; i < batchSize; i++)
                                {
                                    EditorGUILayout.LabelField($"Mint {i + 1}", LabelBoldStyle);
                                    recipients[i] = EditorGUILayout.TextField("Recipient", recipients[i]);
                                    tokenIds[i] = EditorGUILayout.TextField("Token ID", tokenIds[i]);
                                    GUILayout.Space(6);
                                }

                                controller.SafeMintERC721BatchRecipients = recipients;
                                controller.SafeMintERC721BatchTokenIds = tokenIds;

                                if (GUILayout.Button("Mint NFT Contract Batch", ButtonStyle, GUILayout.Height(26)))
                                    controller.SafeMintBatchERC721();
                            }
                            else
                            {
                                controller.SafeMintERC721Recipient = EditorGUILayout.TextField("Recipient Address",
                                    controller.SafeMintERC721Recipient);
                                controller.SafeMintERC721TokenId =
                                    EditorGUILayout.TextField("Token ID", controller.SafeMintERC721TokenId);
                                GUILayout.Space(8);
                                if (GUILayout.Button("Mint NFT Contract", ButtonStyle, GUILayout.Height(26)))
                                    controller.SafeMintERC721();
                            }

                            break;

                        case ContractType.Web3GameItem:
                            controller.MintERC1155ContractAddress =
                                EditorGUILayout.TextField("Item Address", controller.MintERC1155ContractAddress);
                            controller.IsBatchMintERC1155 =
                                EditorGUILayout.Toggle("Batch Mint", controller.IsBatchMintERC1155);
                            GUILayout.Space(8);

                            if (controller.IsBatchMintERC1155)
                            {
                                int batchSize = controller.MintERC1155BatchRecipients?.Length ?? 0;
                                batchSize = EditorGUILayout.IntField("Batch Size", batchSize);
                                batchSize = Mathf.Max(batchSize, 0);
                                GUILayout.Space(6);

                                var recipients = controller.MintERC1155BatchRecipients ?? new string[0];
                                var ids = controller.MintERC1155BatchIds ?? new string[0];
                                var amounts = controller.MintERC1155BatchAmounts ?? new string[0];
                                var datas = controller.MintERC1155BatchDatas ?? new string[0];

                                if (recipients.Length != batchSize) Array.Resize(ref recipients, batchSize);
                                if (ids.Length != batchSize) Array.Resize(ref ids, batchSize);
                                if (amounts.Length != batchSize) Array.Resize(ref amounts, batchSize);
                                if (datas.Length != batchSize) Array.Resize(ref datas, batchSize);

                                for (int i = 0; i < batchSize; i++)
                                {
                                    EditorGUILayout.LabelField($"Mint {i + 1}", LabelBoldStyle);
                                    recipients[i] = EditorGUILayout.TextField("Recipient", recipients[i]);
                                    ids[i] = EditorGUILayout.TextField("ID", ids[i]);
                                    amounts[i] = EditorGUILayout.TextField("Amount", amounts[i]);
                                    datas[i] = EditorGUILayout.TextField("Data", datas[i]);
                                    GUILayout.Space(6);
                                }

                                controller.MintERC1155BatchRecipients = recipients;
                                controller.MintERC1155BatchIds = ids;
                                controller.MintERC1155BatchAmounts = amounts;
                                controller.MintERC1155BatchDatas = datas;

                                if (GUILayout.Button("Mint Web 3 Game Item Batch", ButtonStyle, GUILayout.Height(26)))
                                    controller.MintERC1155Batch();
                            }
                            else
                            {
                                controller.MintERC1155RecipientAddress = EditorGUILayout.TextField("Recipient Address",
                                    controller.MintERC1155RecipientAddress);
                                controller.MintERC1155Id = EditorGUILayout.TextField("ID", controller.MintERC1155Id);
                                controller.MintERC1155Amount =
                                    EditorGUILayout.TextField("Amount", controller.MintERC1155Amount);
                                controller.MintERC1155Data =
                                    EditorGUILayout.TextField("Data", controller.MintERC1155Data);
                                GUILayout.Space(8);
                                if (GUILayout.Button("Mint Web 3 Game Item", ButtonStyle, GUILayout.Height(26)))
                                    controller.MintERC1155();
                            }

                            break;
                    }
                }
            }

            #endregion

            #region Generate SO

            generateFoldout = EditorGUILayout.Foldout(generateFoldout, "Generate SO", true, FoldoutStyle);
            if (generateFoldout)
            {
                using (new EditorGUILayout.VerticalScope(SectionBoxStyle))
                {
                    selectedGenerateType =
                        (ContractType)EditorGUILayout.EnumPopup("Contract Type", selectedGenerateType);
                    GUILayout.Space(8);

                    switch (selectedGenerateType)
                    {
                        case ContractType.CurrencyToken:
                            controller.soContractAddress =
                                EditorGUILayout.TextField("Currency Address", controller.soContractAddress);
                            controller.soTokenDecimals =
                                EditorGUILayout.IntField("Token Decimals", controller.soTokenDecimals);
                            GUILayout.Space(8);

                            if (GUILayout.Button("Create Currency Token ScriptableObject", ButtonStyle,
                                    GUILayout.Height(26)))
                            {
                                var asset = CreateInstance<ERC20Scriptable>();
                                asset.contractAddress = controller.soContractAddress;

                                string path = EditorUtility.SaveFilePanelInProject("Save ERC20 ScriptableObject",
                                    "NewERC20Scriptable", "asset", "Please enter a file name to save the asset to");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    AssetDatabase.CreateAsset(asset, path);
                                    AssetDatabase.SaveAssets();
                                    EditorUtility.FocusProjectWindow();
                                    Selection.activeObject = asset;
                                }
                            }

                            break;

                        case ContractType.NFTContract:
                            controller.soContractAddress =
                                EditorGUILayout.TextField("Contract Address", controller.soContractAddress);
                            GUILayout.Space(8);

                            if (GUILayout.Button("Create NFT Contract ScriptableObject", ButtonStyle,
                                    GUILayout.Height(26)))
                            {
                                var asset = CreateInstance<ERC721Scriptable>();
                                asset.contractAddress = controller.soContractAddress;

                                string path = EditorUtility.SaveFilePanelInProject("Save ERC721 ScriptableObject",
                                    "NewERC721Scriptable", "asset", "Please enter a file name to save the asset to");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    AssetDatabase.CreateAsset(asset, path);
                                    AssetDatabase.SaveAssets();
                                    EditorUtility.FocusProjectWindow();
                                    Selection.activeObject = asset;
                                }
                            }

                            break;

                        case ContractType.Web3GameItem:
                            controller.soContractAddress =
                                EditorGUILayout.TextField("Item Address", controller.soContractAddress);
                            controller.soSpecifyBurnAddress = EditorGUILayout.Toggle("Specify Burn Address",
                                controller.soSpecifyBurnAddress);

                            if (controller.soSpecifyBurnAddress)
                            {
                                controller.soContractBurnAddress =
                                    EditorGUILayout.TextField("Burn Address", controller.soContractBurnAddress);

                            }

                            GUILayout.Space(8);

                            if (GUILayout.Button("Create Web 3 Game Item ScriptableObject", ButtonStyle,
                                    GUILayout.Height(26)))
                            {
                                var asset = CreateInstance<ERC1155Scriptable>();
                                asset.contractAddress = controller.soContractAddress;
                                asset.specifyBurnAddress = controller.soSpecifyBurnAddress;

                                string path = EditorUtility.SaveFilePanelInProject("Save ERC1155 ScriptableObject",
                                    "NewERC1155Scriptable", "asset", "Please enter a file name to save the asset to");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    AssetDatabase.CreateAsset(asset, path);
                                    AssetDatabase.SaveAssets();
                                    EditorUtility.FocusProjectWindow();
                                    Selection.activeObject = asset;
                                }
                            }

                            break;

                    }
                }
            }

            #endregion
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }

    public enum ContractType
    {
        CurrencyToken,
        NFTContract,
        Web3GameItem,
    }

    public enum ContractRole
    {
        Minter
    }
}
