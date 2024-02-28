using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpillQTE : QTEMiniGameSystem
{
   protected void Update()
   {
      if (qteActive = true)
      {
         CheckInput();
         
      }
   }

   public void ActivateQte()
   {
      StartQTE();
   }
   
   protected override void OnQTECompleted()
   {
      base.OnQTECompleted();
      
   }
}
