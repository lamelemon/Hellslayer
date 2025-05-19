using UnityEngine;
using System;
public class ArmsPointActionFunc : MonoBehaviour
{
    public bool FirstAttackPoint = false; // Flag to check if the first attack point in place in animation
    public bool SecondAttackPoint = false; // Flag to check if the second attack point in place in animation
    public bool ThirdAttackPoint = false; // Flag to check if the third attack point in place in animation
    public void FirstAttackPointFunc()
    {
        FirstAttackPoint = true; // Set the flag to true when the first attack point is reached
    }
    public void SecondAttackPointFunc()
    {
        SecondAttackPoint = true; // Set the flag to true when the first attack point is reached
    }
    public void ThirdAttackPointFunc()
    {
        ThirdAttackPoint = true; // Set the flag to true when the first attack point is reached
    }
}
