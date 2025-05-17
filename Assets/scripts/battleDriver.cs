using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using boardPiece;

public class battleDriver : MonoBehaviour
{
    public static Vector2 enabledField;
    public boardGraphic bGraphics;

    public Vector2 thisLocation;
    List<piece> pList;

    void Start(){
        bGraphics = GameObject.Find("playArea").GetComponent<boardGraphic>();
        StartCoroutine(battleLoop());
    }

    delegate IEnumerator UserMove();
    UserMove userMove;
    bool defend, waitingForUser = true;


    IEnumerator battleLoop(){
        Debug.Log("coroutine run");
        WaitForSeconds t = new WaitForSeconds(gameManager.instance.AttackPeriod);
        int multiplier = 2;
        yield return new WaitForSeconds(1);
        // Debug.Log("coroutine started");
        Debug.Log("waited for 1 sec");

        while(true){
        //     // if (enabledField == thisLocation){
        //     //     if (boardDriver.iswhiteTurn){
        //     //         time.TimeScale = 0;
        //     //         // wait for user to make a choice 
        //     //     }

        //     // }
            
            pList = boardDriver.onBoard[thisLocation];
            // Debug.Log($"position = {thisLocation}, count = {pList.Count}")
            for (int i= pList.Count-1; i>=0; i--){
                piece attackingPiece = pList[i];
                
                if (attackingPiece.health>0){
                    if (attackingPiece.isWhite){
                        if (enabledField == thisLocation){
                            while (waitingForUser){
                                yield return null;
                            }
                            yield return StartCoroutine(userMove());
                            waitingForUser = true;
                            continue;
                        }
                    }

                    attack(attackingPiece,multiplier);
                    multiplier =1;
                    yield return t;
                }
            }
            int n =0;
            //removing death pieces
            while (n<pList.Count){
                if (pList[n].health<=0){
                    pList.RemoveAt(n);
                }else n++;
            }
            //checking victory
            if (pList.Count <=1) closeBattle();
            for (int i =0; i<pList.Count-1; i++){
                if (pList[i].isWhite != pList[i+1].isWhite) break;
                closeBattle();
            }
            bGraphics.drawBox(thisLocation);
            yield return null;
        }
        // yield return null;
    }


    void attack(piece attackingPiece, int multiplier){
        foreach(piece pieceToAttack in pList){
            if (attackingPiece.isWhite != pieceToAttack.isWhite){
                pieceToAttack.health -= attackingPiece.attack * multiplier;
                Debug.Log("attacked, -" + attackingPiece.attack * multiplier + "damage");
                // if (pieceToAttack.health < 0){
                //     pList.Remove(pieceToAttack);
                // }
                break;
            }
        }
    }  

    void closeBattle(){
        boardDriver.battleFields.Remove(thisLocation);
        Destroy(gameObject);
    }  

    void onChooseAttack(){

    }

    void onChooseDefend(){

    }

    void onChooseSpecial(){

    }
}
