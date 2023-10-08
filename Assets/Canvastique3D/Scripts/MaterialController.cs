using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canvastique3D
{
    // Contains shader property names to be used throughout the script.
    public static class ShaderProperties
    {
        public const string VideoTexPropertyName = "_VideoTex";
        public const string M0PropertyName = "_M0";
        public const string M1PropertyName = "_M1";
        public const string M2PropertyName = "_M2";
    }

    // Responsible for controlling Material related operations.
    public class MaterialController : MonoBehaviour
    {
        // List of materials to be controlled
        [SerializeField]
        List<Material> materials = new List<Material>();

        // Processed texture to be used
        private Texture2D processedTexture;

        // Sprite array that holds the sprite animation frames
        private Sprite[] spriteFrames;

        // Coroutine reference for sprite animation
        private Coroutine animationCoroutine = null;

        // Sprite animation interval
        [SerializeField]
        private float spriteInterval = 0.05f;

        // Create a 1x1 texture and fill it with white color
        Texture2D blankTexture;


        private void Awake()
        {
            // Load sprite frames from path
            spriteFrames = Resources.LoadAll<Sprite>("Textures/ProcessingVariation");

            blankTexture = new Texture2D(1, 1);
            blankTexture.SetPixel(0, 0, Color.white);
            blankTexture.Apply();
        }

        public void Init()
        {
            ResetTransform();
        }

        // Initializes the processed texture with the specified resolution.
        public void InitializeProcessedTexture(String resolution)
        {
            var width = int.Parse(resolution.Split("x")[0]);
            var height = int.Parse(resolution.Split("x")[1]);

            processedTexture = new Texture2D(width, height);

            Debug.Log($"Material has been initialized with {width}x{height} resolution.");
        }

        // Initializes the material with the given texture.
        public void InitializeMaterial(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.Log("Texture value is null.");
                return;
            }

            OperateOnMaterials(materials, (material) =>
            {
                material.SetTexture(ShaderProperties.VideoTexPropertyName, texture);
            });

            Debug.Log("Material has been initialized.");
        }

        public void ResetMaterial()
        {
            OperateOnMaterials(materials, (material) =>
            {
                material.SetTexture(ShaderProperties.VideoTexPropertyName, blankTexture);
            });
        }

        // Get and Set for the processed texture.
        public Texture2D ProcessedTexture
        {
            get
            {
                return processedTexture;
            }
            set
            {
                processedTexture = value;
            }
        }

        // Sets the transformation matrix in the shader assigned to each material
        public void SetTransform(double[] M)
        {
            OperateOnMaterials(materials, (material) =>
            {
                material.SetVector(ShaderProperties.M0PropertyName, new Vector3((float)M[0], (float)M[1], (float)M[2]));
                material.SetVector(ShaderProperties.M1PropertyName, new Vector3((float)M[3], (float)M[4], (float)M[5]));
                material.SetVector(ShaderProperties.M2PropertyName, new Vector3((float)M[6], (float)M[7], 1f));
            });

            SaveTransform(M);

            Debug.Log("Perspective Warped.");
        }

        // Resets the transformation matrix in the shader assigned to each material
        public void ResetTransform()
        {
            OperateOnMaterials(materials, (material) =>
            {
                material.SetVector(ShaderProperties.M0PropertyName, new Vector3(1f, 0f, 0f));
                material.SetVector(ShaderProperties.M1PropertyName, new Vector3(0f, 1f, 0f));
                material.SetVector(ShaderProperties.M2PropertyName, new Vector3(0f, 0f, 1f));
            });

            Debug.Log("Perspective Reset.");
        }

        // Performs an operation on the given list of materials.
        private void OperateOnMaterials(List<Material> materials, Action<Material> operation)
        {
            if (materials != null && materials.Count > 0)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] != null)
                    {
                        operation(materials[i]);
                    }
                }
            }
            else
            {
                Debug.LogError("No materials assigned!");
            }
        }

        // Saves the transformation matrix to PlayerPrefs.
        private void SaveTransform(double[] data)
        {
            string dataStr = string.Join(",", data.Select(d => d.ToString()).ToArray());
            PlayerPrefs.SetString("MaterialTransform", dataStr);

            Debug.Log("Transform saved: " + dataStr);
        }

        // Loads the transformation matrix from PlayerPrefs.
        public void LoadTransform()
        {
            if (!PlayerPrefs.HasKey("MaterialTransform"))
            {
                Debug.Log("No saved transform data available.");
                return;
            }

            string dataStr = PlayerPrefs.GetString("MaterialTransform");
            string[] splitData = dataStr.Split(',');

            float[] data = new float[splitData.Length];

            for (int i = 0; i < splitData.Length; i++)
            {
                float.TryParse(splitData[i], out data[i]);
            }

            OperateOnMaterials(materials, (material) =>
            {
                material.SetVector(ShaderProperties.M0PropertyName, new Vector3(data[0], data[1], data[2]));
                material.SetVector(ShaderProperties.M1PropertyName, new Vector3(data[3], data[4], data[5]));
                material.SetVector(ShaderProperties.M2PropertyName, new Vector3(data[6], data[7], 1f));
            });

            Debug.Log("Transform loaded.");
        }

        // Displays a sprite by looping through a sprite array at a certain interval and assigning each sprite to a material.
        public void PlaySpriteAnimation()
        {
            // Stop any existing animation
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            // Start a new animation
            animationCoroutine = StartCoroutine(AnimateSprite());
        }

        // Animate sprite routine
        private IEnumerator AnimateSprite()
        {
            while(true)
            {
                foreach (Sprite sprite in spriteFrames)
                {
                    // Convert sprite to Texture2D
                    Texture2D texture = SpriteToTexture2D(sprite);

                    // Initialize the material with the sprite texture
                    InitializeMaterial(texture);

                    // Wait for the next frame
                    yield return new WaitForSeconds(spriteInterval);
                }
            }
        }

        // Converts a Sprite to Texture2D
        private Texture2D SpriteToTexture2D(Sprite sprite)
        {
            Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                  (int)sprite.textureRect.y,
                                                  (int)sprite.textureRect.width,
                                                  (int)sprite.textureRect.height);
            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        // Stops the sprite animation
        public void StopSpriteAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }

    }
}
