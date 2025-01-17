using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image[] _circles;
        [SerializeField] [Range(0, 255)] private int _maxAlpha = 255;
        [SerializeField] [Range(0, 255)] private int _minAlpha = 0;
        [SerializeField] private float _animationSpeed = .1f;
        
        private const int DefaultFullAlphaValue = 255;
        
        private void OnEnable()
        {
            StartCoroutine(LoadingAnimation());
        }

        private IEnumerator LoadingAnimation()
        {
            int numberOfCircles = _circles.Length;
            float[] possibleCircleAlphaValues = GetPossibleCircleAlphaValues(numberOfCircles);
            int[] circleAlphaValueIndices = GetRandomStartingIndex(numberOfCircles);

            while (true)
            {
                for (int i = 0; i < numberOfCircles; i++)
                {
                    Color currentColor = _circles[i].color;
                    float newAlpha = possibleCircleAlphaValues[circleAlphaValueIndices[i]];
                    float asRGBA = newAlpha / DefaultFullAlphaValue;
                    _circles[i].color = new Color(currentColor.r, currentColor.g, currentColor.b, asRGBA);
                }
                
                yield return new WaitForSeconds(_animationSpeed);

                circleAlphaValueIndices = ShiftIndices(circleAlphaValueIndices);
            }
        }

        private float[] GetPossibleCircleAlphaValues(int numberOfCircles)
        {
            float[] possibleCircleAlphaValues = new float[numberOfCircles];
            float incrementAmount = (float) (_maxAlpha - _minAlpha) / numberOfCircles;
            for (int i = 0; i < numberOfCircles; i++)
            {
                possibleCircleAlphaValues[i] = _minAlpha + (incrementAmount * i);
            }

            return possibleCircleAlphaValues;
        }

        private int[] GetRandomStartingIndex(int numberOfCircles)
        {
            int[] circleAlphaValueIndices = new int[numberOfCircles];
            int randomStartingPoint = UnityEngine.Random.Range(0, numberOfCircles);
            Queue<int> indicesQueue = new Queue<int>();
            for (int i = 0; i < numberOfCircles; i++)
            {
                indicesQueue.Enqueue(i);
            }
            for (int i = randomStartingPoint; i < numberOfCircles; i++)
            {
                circleAlphaValueIndices[i] = indicesQueue.Dequeue();
            }
            for (int i = 0; i < randomStartingPoint; i++)
            {
                circleAlphaValueIndices[i] = indicesQueue.Dequeue();
            }

            return circleAlphaValueIndices;
        }

        private int[] ShiftIndices(int[] indicesArray)
        {
            int length = indicesArray.Length;
            int[] shifted = new int[length];
            for (int i = 0; i < length; i++)
            {
                shifted[i] = indicesArray[i] - 1;
                if (shifted[i] == -1)
                {
                    shifted[i] = length - 1;
                }
            }

            return shifted;
        }
    }
}