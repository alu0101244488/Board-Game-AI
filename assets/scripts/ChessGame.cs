using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public class ChessGame : MonoBehaviour
{
    [SerializeField] ChessEnemy enemy;
    [SerializeField] ChessBoardBehavoir boardBehavoir;

    //int turn = 0;
    public static int depth = 1;
    public string fenPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    //bool GameOver = false;
    //bool turnPlayer1 = true;
  
    public static int delayConstant = 100;

    public static int delay;

    // record last movement (last board position and player turn)
    //dificultad
        //facil, elige la peor decisión
        //media, elige una desicion media
        //dificil, elige la mejor desicion
        //de entre todas las que tienen el mismo valor elige una al azar



    void Awake() {
        delay = delayConstant;
    }

    void Update() {
        //if(GameOver) {
            //terminar
        //}
    }

    public async void changeTurn(ChessPieceBehavoir lastPieceMoved, int newColumn, int newRow) {
        boardBehavoir = this.GetComponent<ChessBoardBehavoir>();
        // check if player pawn at last row to change to queen
        /*for(int i = 0; i < boardBehavoir.gridSize; i++) {
            if(boardBehavoir.pieceAtpos(i, boardBehavoir.gridSize - 1)) {
                if(boardBehavoir.getPieceAtPos(i,  boardBehavoir.gridSize - 1).Type == PieceType.Pawn) {
                    //Debug.Log(boardBehavoir.getPieceAtPos(i, boardBehavoir.gridSize - 1).Type);
                    boardBehavoir.deletePieceAtPos(i, boardBehavoir.gridSize - 1);
                    //boardBehavoir.addPieceAtPos(i + 1, boardBehavoir.gridSize - 1, PieceColor.White, PieceType.Queen);
                }
            }
        }*/


        //deactivate pieces 
       
        //if piece moved is pawn and is on last row || else if is enemy pawn and is on first row
        //boadbehavoir.delete(piece.column, piece.row)
        //board.addPiece(queen.)


        Debug.Log("delay: " + delay + "  depth: " + depth);

        await Task.Delay(delay);
        enemy.moveMinMax(depth);
        //turn++;
        // if any king is under attack an eevery next move is under attack, delete every dragdrop
        // of all player pieces

    }


    // if next move any king is under attack
    bool checkmate() {return false;}

    public void restart() {
        boardBehavoir.destroyPieces();
        fenPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        depth = 1;

        ChessEnemy.playerPawnValue = -10;
        ChessEnemy.playerKnightValue = -30;
        ChessEnemy.playerRookValue = -50;
        ChessEnemy.playerBishopValue = -30;
        ChessEnemy.playerQueenValue = -90;
        ChessEnemy.playerKingValue = -900;
        ChessEnemy.enemyPawnValue = 10;
        ChessEnemy.enemyKnightValue = 30;
        ChessEnemy.enemyRookValue = 50;
        ChessEnemy.enemyBishopValue = 30;
        ChessEnemy.enemyQueenValue = 90;
        ChessEnemy.enemyKingValue = 900;
    }

    public void startChessGame() {
        boardBehavoir.destroyPieces();
        boardBehavoir.GenerateBoard(fenPosition);
        enemy.SetEnemyPieces(PieceColor.Black);
    }

    public void setNewFenBoard(string newFenString) {
        fenPosition = newFenString;
        Debug.Log("fen string: " + newFenString );
    }


    public void changeDepth(string strNewDepth) {
        // try parse new depth ton int
        int newDepth = 1;
        if(Int32.TryParse(strNewDepth, out newDepth)) {depth = newDepth;}
        //depth = newDepth;
        if(depth == 0) {delay = 100;}
        else {delay = delayConstant / depth;}
        Debug.Log("button " + depth);
    }

    //public void changePlayerColor() {}

    public void exitButton() {
        Application.Quit();
    }
    
}


