using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using boardPiece;
using System.Threading;
using System.Threading.Tasks;


public class boardDriver : MonoBehaviour
{
    // [SerializeField]
    boardGraphic bGraphics;

    [SerializeField]
    int botLevel = 4;
    // public static float botClockPeriod = 2f;
    [SerializeField]
    bool isBlackBot = true, isWhiteBot = true, normalChess;

    public static Dictionary<Vector2, List<piece>> onBoard;
    public static List<Vector2> battleFields;
    public static bool isWhiteTurn;
    piece whiteKing;
    piece blackKing;


    void Awake(){
        battleFields = new List<Vector2>();
        bGraphics = GetComponent<boardGraphic>();
        readFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        // readFEN("8/1p6/1n6/8/2P5/8/8/8");
        // readFEN("rnbqkbnr/pp1ppppp/2p5/4p3/8/2N2N2/PPPPPPPP/R1BQKB1R");
        isWhiteTurn = true;
    }

    void Start(){
        // miniMax(onBoard, botLevel, isWhiteTurn, -999, 999, out Move bestMove);

        StartCoroutine(gameLoop());
    }

    void tempfxn(List<Vector2> bField){
        bField.Add(new Vector2(1,1));
    }

    // void Update(){
    //     if (Input.GetKeyDown("n")){
    //         miniMax();
    //     }
    // }

    public void makeMove(Move move, Dictionary<Vector2, List<piece>> OB, List<Vector2> bFields){
        Vector2 dest = move.dest;
        Vector2 origin = move.origin;
        piece p = move.pieceToMove;

        if (OB[origin].Count == 1){
            OB.Remove(origin);
        }else{
            OB[origin].Remove(p);
        }

        if (OB.TryGetValue(dest, out List<piece> pList)){
            if(normalChess){
                pList[0] = p;
            }else{
                pList.Add(p);
                // pList[0] = p;
                bFields.Add(dest);
            }

        }else{
            OB.Add(dest, new List<piece>(){p});
        }

        p.pos = dest;

        // bGraphics.movePiece(move);
    }


    public void makeMoveOnBoard(Move move, Dictionary<Vector2, List<piece>> OB){
        Vector2 dest = move.dest;
        Vector2 origin = move.origin;
        piece p = move.pieceToMove;

        if (OB[origin].Count == 1){
            OB.Remove(origin);
        }else{
            OB[origin].Remove(p);
        }

        if (OB.TryGetValue(dest, out List<piece> pList)){
            if(normalChess){
                pList[0] = p;
            }else{
                pList.Add(p);
                battleFields.Add(dest);
                bGraphics.MakeBattleField(dest);
            }

        }else{
            OB.Add(dest, new List<piece>(){p});
        }

        bGraphics.movePiece(move);
        isWhiteTurn = !isWhiteTurn;
    }


    float calcPoint(Move move, Dictionary<Vector2, List<piece>> onBoard, bool isWhiteTurn){
        Vector2 dest = move.dest;
        Vector2 origin = move.origin;
        piece pieceToMove = move.pieceToMove;

        float capturePoint = 0;
        if (onBoard.TryGetValue(dest, out List<piece> pList)){
            foreach (piece p in pList){
                if (p.isWhite != isWhiteTurn){
                    capturePoint = Mathf.Max(capturePoint,p.value);
                }
            }
        }
        float CentreControl = (new Vector2(3.5f,3.5f) - origin).magnitude - (new Vector2(3.5f,3.5f) - dest).magnitude;
        CentreControl = CentreControl/4 * pieceToMove.centreAfinity * onBoard.Count/32;

        float points = capturePoint + CentreControl;

        if (pieceToMove.value < 10 && onBoard.Count > 10) return points;

        // float endgamePoint = (10 - (whiteKing.pos - blackKing.pos).magnitude)/8;
        // endgamePoint = (new Vector2(3.5f,3.5f) - origin).magnitude - (new Vector2(3.5f,3.5f) - dest).magnitude;

        // endgamePoint += ((pieceToMove.isWhite ? blackKing.pos : whiteKing.pos) - new Vector2(3.5f,3.5f)).magnitude/5;

        return points;// + endgamePoint;
    }


    List<Move> generateMoves(Dictionary<Vector2, List<piece>> onBoard, bool isWhiteTurn){
        List<Move> moves = new List<Move>();
        foreach (KeyValuePair<Vector2, List<piece>> ele in onBoard){
            foreach ( piece p in ele.Value){ 
                if (p.isWhite != isWhiteTurn) continue;
                
                p.calculateMove(onBoard, battleFields, ele.Key);
                foreach (Vector2 dest in p.moveList){
                    Move move;
                    move.dest = dest;
                    move.origin = ele.Key;
                    move.pieceToMove = p;
                    // move p to dest virtually
                    Dictionary<Vector2, List<piece>> onBoardCopy = cloneDictionary(onBoard);
                    makeMove(move, onBoardCopy, battleFields);
        // Debug.Log($"returning move list, move count: {moves.Count}");

                    if (isChecked(onBoardCopy, !p.isWhite)){
                        continue;
                    }

                    moves.Add(move);
                }
            }
        }
        return moves;
    }


    public bool isChecked(Dictionary<Vector2, List<piece>> onBoard, bool isWhiteTurn){
        // List<Move> moves = new List<Move>();
        foreach (KeyValuePair<Vector2, List<piece>> ele in onBoard){
            foreach ( piece p in ele.Value){ 
                if (p.isWhite != isWhiteTurn) continue;
                
                p.calculateMove(onBoard, battleFields, ele.Key);
                foreach (Vector2 dest in p.moveList){
                    if (onBoard.TryGetValue(dest, out List<piece> pList)){
                        foreach(piece capturingPiece in pList){
                            if(capturingPiece.value > 10){
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
}


    void readFEN(string fen){
        onBoard = new Dictionary<Vector2, List<piece>>();

        string[] fenLines = fen.Split("/");
        for (int i=0; i<8; i++){
            int rank = 7-i;
            int file = 0;
            foreach (char C in fenLines[i]){
                string c = char.ToString(C);
                if(int.TryParse(c, out int n)){
                    file += n;
                    continue;
                }

                Vector2 pos = new Vector2(file, rank);
                switch (c){
                    case "p":
                        onBoard.Add(pos, new List<piece>(){new pawn(pos, false)});
                        break;
                    case "b":
                        onBoard.Add(pos, new List<piece>(){new bishop(pos, false)});
                        break;
                    case "n":
                        onBoard.Add(pos, new List<piece>(){new knight(pos, false)});
                        break;
                    case "r":
                        onBoard.Add(pos, new List<piece>(){new rook(pos, false)});
                        break;
                    case "q":
                        onBoard.Add(pos, new List<piece>(){new queen(pos, false)});
                        break;
                    case "k":
                    piece p = new king(pos, false);
                        onBoard.Add(pos, new List<piece>(){p});
                        blackKing = p;
                        break;
                    case "P":
                        onBoard.Add(pos, new List<piece>(){new pawn(pos, true)});
                        break;
                    case "B":
                        onBoard.Add(pos, new List<piece>(){new bishop(pos, true)});
                        break;
                    case "N":
                        onBoard.Add(pos, new List<piece>(){new knight(pos, true)});
                        break;
                    case "R":
                        onBoard.Add(pos, new List<piece>(){new rook(pos, true)});
                        break;
                    case "Q":
                        onBoard.Add(pos, new List<piece>(){new queen(pos, true)});
                        break;
                    case "K":
                        p = new king(pos, true);
                        onBoard.Add(pos, new List<piece>(){p});
                        whiteKing = p;
                        break;
                    default:
                        break;
                }
                file++;   
            }
        } 
    }


    bool SearchCancel = false;

    void miniMax(){
        miniMax(onBoard, battleFields, botLevel, isWhiteTurn, -999, 999, out bestMove);
        makeMoveOnBoard(bestMove,onBoard);
    }

    float miniMax(Dictionary<Vector2, List<piece>> OB, List<Vector2> battleField, int depth, bool isWhiteTurn, float alpha, float beta, out Move bestMove){
        bestMove = default;
        if (SearchCancel) return 0;
        if (depth == 0) return 0;

        //stalemate()
        // Dictionary<Vector2, List<piece>> onBoardCopy1 = cloneDictionary(OB);
        // writeDic(onBoardCopy1);

        List<Move> moves = generateMoves(OB, isWhiteTurn);

        if(moves.Count == 0) {
            Debug.Log("no move left");
            return 0;
        }

        List<Vector2> battleFieldCopy = new List<Vector2>();
        for (int i=0; i<battleFields.Count; i++){
            battleFieldCopy.Add(battleFields[i]);
        }
        

        if (isWhiteTurn){
            float maxPoint = -999;
            foreach(Move move in moves){
                Dictionary<Vector2, List<piece>> onBoardCopy = cloneDictionary(OB);
                float point = calcPoint(move, onBoardCopy, isWhiteTurn);
                makeMove(move, onBoardCopy, battleFields);
                point += miniMax(onBoardCopy, battleFieldCopy, depth-1, !isWhiteTurn, alpha, beta, out  Move m);
                if (point > maxPoint){
                    maxPoint = point;
                    bestMove = move;
                }
                maxPoint = Mathf.Max(maxPoint,point);
                alpha = Mathf.Max(alpha, point);
                if (beta <= alpha){
                    break;
                }
            }
            return maxPoint;
        }else{
            float minPoint = 999;
            foreach(Move move in moves){
                Dictionary<Vector2, List<piece>> onBoardCopy = cloneDictionary(OB);
                float point = -calcPoint(move, onBoardCopy, isWhiteTurn);
                makeMove(move, onBoardCopy, battleFields);
                point += miniMax(onBoardCopy, battleFieldCopy, depth-1, !isWhiteTurn, alpha, beta, out  Move m);
                if (point < minPoint){
                    minPoint = point;
                    bestMove = move;
                }
                beta = Mathf.Min(beta, point);
                if (beta <= alpha){
                    break;
                }
            }
            return minPoint;
        }
    }

    Move bestMove;
    async Task StartMiniMaxAsync()
    {
        bool _isWhiteTurn = isWhiteTurn;
        Dictionary<Vector2, List<piece>> _onBoard = cloneDictionary(onBoard);
        await Task.Run(() => {
            // dummyfxn();
            miniMax(_onBoard, battleFields, botLevel, _isWhiteTurn, -999, 999, out Move _bestMove);
            bestMove = _bestMove;
        });
    }

    IEnumerator gameLoop(){
        yield return new WaitForSeconds(1);

        boardGraphic.gameLoopStarted = true;
        // yield return new WaitForSeconds(0.5f);
        WaitForSeconds delay = new WaitForSeconds(gameManager.instance.BoardPeriod);
        while (true){
            if(gameManager.instance.GameOver) break;

            boardGraphic.progress = 0;

            if (isWhiteTurn ? !isWhiteBot : !isBlackBot){
                yield return null;
                continue;
            }

            Task taskCalculateMove = StartMiniMaxAsync();

            yield return delay;

            yield return new WaitUntil(() => taskCalculateMove.IsCompleted);
            // Debug.Log($"taskCalculateMove.IsCompleted: {taskCalculateMove.IsCompleted}");
            // printMove(bestMove);
            makeMoveOnBoard(bestMove,onBoard);
        }
    }

    void printMove(Move move){
        Debug.Log($"move origin: {move.origin}, dest: {move.dest}, piece: {move.pieceToMove}");
    }

    void writeDic(Dictionary<Vector2, List<piece>> onBoard){
        foreach (KeyValuePair<Vector2, List<piece>> ele in onBoard){
            Debug.Log("pieces: " + ele.Value[0] + ele.Key);
        }
    }

    public Dictionary<Vector2, List<piece>> cloneDictionary(Dictionary<Vector2, List<piece>> dic){
        Dictionary<Vector2, List<piece>> newDic = new Dictionary<Vector2, List<piece>>();
        foreach (KeyValuePair<Vector2, List<piece>> ele in dic){
            List<piece> newPieceList = new List<piece>();
            foreach (piece p in ele.Value){
                piece newPiece = p.clonePiece();
                newPieceList.Add(newPiece);
            }
            newDic.Add(ele.Key, newPieceList);
        }
        return newDic;
    }

}
