using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace boardPiece
{
public struct Move{
    public Vector2 dest;
    public Vector2 origin;
    public piece pieceToMove;
}

public abstract class piece
{
    public Vector2 pos;
    public bool isWhite;
    public float health;
    public readonly float attack;
    public readonly float value;
    public readonly float centreAfinity;

    public List<Vector2> moveList;

    public piece(float value, float centreAfinity, float health, float attack){
        this.value = value;
        this.centreAfinity = centreAfinity;
        this.attack = attack;
        this.health = health;
    }

    public int bti(bool x){ //bool to int
        return x ? 1 : 0;
    }

    public bool inBound(Vector2 pos){
        if (pos.x<0 || pos.x>7)return false;
        if (pos.y<0 || pos.y>7)return false;
        return true; 
    }

    public abstract void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos);

    public abstract piece clonePiece();
}

public class pawn : piece{
    public pawn(Vector2 pos, bool white) : base(1,1,5,1){
        this.pos = pos;
        this.isWhite = white;
    }


    public override void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos){
        moveList = new List<Vector2>();

        if (battleFields.Contains(pos)) return;

        int dir = isWhite ? 1 : -1;

        if (!onBoard.ContainsKey(pos + new Vector2(0,dir))){
            moveList.Add(pos + new Vector2(0,dir));
            if (pos.y == 1*bti(isWhite) + 6*bti(!isWhite)){
                if (!onBoard.ContainsKey(pos + new Vector2(0,2*dir))){
                    moveList.Add(pos + new Vector2(0,2*dir));
                }
            }
        }
        if (pos.x>0 && onBoard.TryGetValue(pos + new Vector2(-1,dir), out List<piece> pieceList)){
            foreach(piece p in pieceList){
                if (p.isWhite != this.isWhite) moveList.Add(pos + new Vector2(-1,dir));
                break;
            }
        }
        if (pos.x<7 && onBoard.TryGetValue(pos + new Vector2(1,dir), out pieceList)){
            foreach(piece p in pieceList){
                if (p.isWhite != this.isWhite) moveList.Add(pos + new Vector2(1,dir));
            }
        }            
    }

    public override piece clonePiece()
    {
        return (new pawn(this.pos, this.isWhite));
    }
}

public class knight : piece{
    public knight(Vector2 pos, bool white) : base(3,1f,15,3){
        this.pos = pos;
        this.isWhite = white;
    }

    public override void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos){
        moveList = new List<Vector2>();

        if (battleFields.Contains(pos)) return;


        Vector2[] possbileMoves = {
            new Vector2(1,2),
            new Vector2(2,1),
            new Vector2(-1,2),
            new Vector2(-2,1),
            new Vector2(1,-2),
            new Vector2(2,-1),
            new Vector2(-1,-2),
            new Vector2(-2,-1)
        };
        Vector2 movePos;
        foreach(Vector2 p in possbileMoves){
            movePos = pos + p;
            if (inBound(movePos)){
                if (onBoard.TryGetValue(movePos, out List<piece> pList)){
                    foreach(piece capturePiece in pList){
                        if (capturePiece.isWhite != this.isWhite) {
                            moveList.Add(movePos);
                            break;
                        }
                    } 
                } else moveList.Add(movePos);
            }
        }
    }

    public override piece clonePiece()
    {
        return (new knight(this.pos, this.isWhite));
    }
}

public class king : piece{
    public king(Vector2 pos, bool white) : base (999,-0.5f, 0.1f, 999){
        this.pos = pos;
        this.isWhite = white;
    }

    public override void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos){
        moveList = new List<Vector2>();

        if (battleFields.Contains(pos)) return;
        // cannot walk into battle field

        Vector2[] possbileMoves = {
            new Vector2(1,1),
            new Vector2(0,1),
            new Vector2(-1,1),
            new Vector2(-1,0),
            new Vector2(1,0),
            new Vector2(1,-1),
            new Vector2(-1,-1),
            new Vector2(0,-1)
        };

        foreach(Vector2 p in possbileMoves){
            Vector2 movePos = pos + p;
            if (inBound(movePos)){
                if (onBoard.TryGetValue(movePos, out List<piece> pList)){
                    foreach(piece capturePiece in pList){
                        if (capturePiece.isWhite != this.isWhite) {
                            moveList.Add(movePos);
                            break;
                        }
                    } 
                } else moveList.Add(movePos);
            }
        }
    }

    public override piece clonePiece()
    {
        return (new king(this.pos, this.isWhite));
    }
}

public class bishop : piece{
    public bishop(Vector2 pos, bool white) : base(3.1f,0.5f,15,3){
        this.pos = pos;
        this.isWhite = white;
    }

    public override void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos)
    {
        moveList = new List<Vector2>();

        if (battleFields.Contains(pos)) return;

        Vector2[] moveDirections = {
            new Vector2(1,1),
            new Vector2(-1,1),
            new Vector2(1,-1),
            new Vector2(-1,-1)
        };

        foreach (Vector2 dir in moveDirections){
            for (int i=1; i<8; i++){
                Vector2 movePos = pos+dir*i;
                if (!inBound(movePos)) break;

                if (onBoard.TryGetValue(movePos, out List<piece> pList)){
                    foreach(piece capturePiece in pList){
                        if (capturePiece.isWhite != this.isWhite) {
                            moveList.Add(movePos);
                            break;
                        }
                    } 
                    break;
                } else moveList.Add(movePos);   
            }
        }   
    }

    public override piece clonePiece()
    {
        return (new bishop(this.pos, this.isWhite));
    }
}

public class rook : piece{
    public rook(Vector2 pos, bool white) : base(5,0,25,5){
        this.pos = pos;
        this.isWhite = white;
    }

    public override void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos)
    {
        moveList = new List<Vector2>();

        if (battleFields.Contains(pos)) return;

        Vector2[] moveDirections = {
            new Vector2(0,1),
            new Vector2(0,-1),
            new Vector2(1,0),
            new Vector2(-1,0)
        };

        foreach (Vector2 dir in moveDirections){
            for (int i=1; i<8; i++){
                Vector2 movePos = pos+dir*i;
                if (!inBound(movePos)) break;

                if (onBoard.TryGetValue(movePos, out List<piece> pList)){
                    foreach(piece capturePiece in pList){
                        if (capturePiece.isWhite != this.isWhite) {
                            moveList.Add(movePos);
                            break;
                        }
                    } 
                    break;
                } else moveList.Add(movePos);   
            }
        }   
    }
    public override piece clonePiece()
    {
        return (new rook(this.pos, this.isWhite));
    }
}

public class queen : piece{
    public queen(Vector2 pos, bool white) : base(9,0,45,9){
        this.pos = pos;
        this.isWhite = white;
    }

    public override void calculateMove(Dictionary<Vector2, List<piece>> onBoard, List<Vector2> battleFields, Vector2 pos)
    {
        moveList = new List<Vector2>();

        if (battleFields.Contains(pos)) return;

        Vector2[] moveDirections = {
            new Vector2(1,1),
            new Vector2(-1,1),
            new Vector2(1,-1),
            new Vector2(-1,-1),
            new Vector2(0,1),
            new Vector2(0,-1),
            new Vector2(1,0),
            new Vector2(-1,0)
        };

        foreach (Vector2 dir in moveDirections){
            for (int i=1; i<8; i++){
                Vector2 movePos = pos+dir*i;
                if (!inBound(movePos)) break;

                if (onBoard.TryGetValue(movePos, out List<piece> pList)){
                    foreach(piece capturePiece in pList){
                        if (capturePiece.isWhite != this.isWhite) {
                            moveList.Add(movePos);
                            break;
                        }
                    } 
                    break;
                } else moveList.Add(movePos);   
            }
        }   
    }

    public override piece clonePiece()
    {
        return (new queen(this.pos, this.isWhite));
    }
}

}

