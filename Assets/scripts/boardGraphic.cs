using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using boardPiece;

public class boardGraphic : MonoBehaviour
{
    public GameObject W_pawn;
    public GameObject W_rook;
    public GameObject W_bishop;
    public GameObject W_knight;
    public GameObject W_queen;
    public GameObject W_king;

    public GameObject B_pawn;
    public GameObject B_rook;
    public GameObject B_bishop;
    public GameObject B_knight;
    public GameObject B_queen;
    public GameObject B_king;


    public Material progessBar;
    boardDriver bDriver;

    Dictionary<Vector2,List<GameObject>> prefabDic;
    float boxSize;
    Vector2 boardOrigin;

    void Start(){
        bDriver = GetComponent<boardDriver>();

        Rect boardRect = GetComponent<RectTransform>().rect;
        boardOrigin = boardRect.position;
        boxSize = boardRect.size.x/8;

        prefabDic = new Dictionary<Vector2, List<GameObject>>();
        drawBoard();
    }

    public static bool gameLoopStarted;
    public static float progress;
    void Update(){
        // board turn progress bar
        if (!gameLoopStarted) return;
        progress += Time.deltaTime/gameManager.instance.BoardPeriod;
        progessBar.SetFloat("_Progress", progress);

        Rect boardRect = GetComponent<RectTransform>().rect;
        boardOrigin = boardRect.position;
        boxSize = boardRect.size.x/8;
    }

    public void movePiece(Move move){
        drawBox(move.origin);
        drawBox(move.dest);
    }

    void drawBoard(){
        foreach(KeyValuePair<Vector2, List<piece>> ele in boardDriver.onBoard){
            drawBox(ele.Key);
        }

    }

    public void drawBox(Vector2 pos){
        if (prefabDic.TryGetValue(pos, out List<GameObject> objs)){
            foreach (GameObject obj in objs){
                Destroy(obj);
            }
            prefabDic.Remove(pos);
        }
        objs = new List<GameObject>();

        if (boardDriver.onBoard.TryGetValue(pos, out List<piece> pList)){
            int n = pList.Count;
            float pieceScale = n>1 ? 0.5f : 1;
            float colorOfset = n>1? 0.5f : 0;
            int i =1;
            foreach (piece p in pList){
                GameObject InstantiatedPiece = instantiatePiece(p, this.transform);
                InstantiatedPiece.transform.localScale = Vector3.one*pieceScale;
                InstantiatedPiece.transform.localPosition = boardOrigin + pos*boxSize + new Vector2((i++)*boxSize/(1+n), boxSize*0.5f*pieceScale + (boxSize*colorOfset)*(p.isWhite? 0 : 1)); 
                objs.Add(InstantiatedPiece);     
            }

            prefabDic.Add(pos, objs);
        }
    }

    GameObject instantiatePiece(piece p, Transform parent){
        GameObject instantiatedPiece;
        switch(p.value * (p.isWhite? 1 : -1)){
            case 1:
                instantiatedPiece = Instantiate(W_pawn,parent);
                break;
            case 3:
                instantiatedPiece = Instantiate(W_knight,parent);
                break;
            case 3.1f:
                instantiatedPiece = Instantiate(W_bishop,parent);
                break;
            case 5:
                instantiatedPiece = Instantiate(W_rook,parent);
                break;
            case 9:
                instantiatedPiece = Instantiate(W_queen,parent);
                break;
            case -1:
                instantiatedPiece = Instantiate(B_pawn,parent);
                break;
            case -3:
                instantiatedPiece = Instantiate(B_knight,parent);
                break;
            case -3.1f:
                instantiatedPiece = Instantiate(B_bishop,parent);
                break;
            case -5:
                instantiatedPiece = Instantiate(B_rook,parent);
                break;
            case -9:
                instantiatedPiece = Instantiate(B_queen,parent);
                break;
            default:
                if(p.isWhite){
                    instantiatedPiece = Instantiate(W_king,parent);
                }else instantiatedPiece = Instantiate(B_king,parent);
                break;               
        }
        return instantiatedPiece;
    }


    public GameObject BattleField;

    public void MakeBattleField(Vector2 location){
        GameObject battleField = Instantiate(BattleField,this.transform);
        battleField.transform.localPosition = boardOrigin + location*boxSize;
        battleField.GetComponent<battleDriver>().thisLocation = location;
    }


    public GameObject MoveableTile;
    List<Vector2> moveList;

    public void PickPiece(Vector2 pickPosition){

        Vector2 origin = Vector2Int.FloorToInt((pickPosition - boardOrigin)/boxSize);
        piece p = boardDriver.onBoard[origin][0];

        moveList = getMoveList(p, origin);

        foreach(Vector2 pos in moveList){
            GameObject battleField = Instantiate(MoveableTile,this.transform);
            battleField.transform.localPosition = boardOrigin + pos*boxSize;
        }
    }

    public void dropPiece(Vector2 pickPosition, Vector2 dropPosition){
        destroyAllTiles("playableTile");
        //get drop position 
        Vector2 origin = Vector2Int.FloorToInt((pickPosition - boardOrigin)/boxSize);
        Vector2 dest = Vector2Int.FloorToInt((dropPosition - boardOrigin)/boxSize);
        piece p = boardDriver.onBoard[origin][0];
        // parse move
        Move move;
        move.pieceToMove = p;
        move.origin = origin;
        move.dest = dest;

        // //check if possible 
        // if (p.isWhite != boardDriver.isWhiteTurn){
        //     drawBox(origin);
        //     return;
        // }

        // // p.calculateMove(boardDriver.onBoard, boardDriver.battleFields, origin);

        // if (!p.moveList.Contains(dest)){
        //     drawBox(origin);
        //     return;            
        // }

        // Dictionary<Vector2, List<piece>> onBoardCopy = bDriver.cloneDictionary(boardDriver.onBoard);
        // bDriver.makeMove(move, onBoardCopy);

        // if (bDriver.isChecked(onBoardCopy, !p.isWhite)){
        //     Debug.Log("your king will be in danger");
        //     drawBox(origin);
        //     return;   
        // }
        if (!moveList.Contains(dest)) drawBox(origin);
        else bDriver.makeMoveOnBoard(move, boardDriver.onBoard);        
    }

    List<Vector2> getMoveList(piece p, Vector2 origin){
        List<Vector2> moveList = new List<Vector2>();


        if (p.isWhite != boardDriver.isWhiteTurn){
            return moveList ;
        }

        p.calculateMove(boardDriver.onBoard, boardDriver.battleFields, origin);

        foreach(Vector2 pos in p.moveList){
            Move move;
            move.pieceToMove = p;
            move.origin = origin;
            move.dest = pos;

            Dictionary<Vector2, List<piece>> onBoardCopy = bDriver.cloneDictionary(boardDriver.onBoard);
            bDriver.makeMove(move, onBoardCopy, boardDriver.battleFields);

            if (!bDriver.isChecked(onBoardCopy, !p.isWhite)){
                moveList.Add(pos);
            }
        }

        return moveList;

    }

    void destroyAllTiles(string s){
        GameObject[] tiles = GameObject.FindGameObjectsWithTag(s);
        foreach(GameObject tile in tiles){
            Destroy(tile);
        }
    }

}
