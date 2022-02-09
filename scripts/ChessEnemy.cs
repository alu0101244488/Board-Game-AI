using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ChessEnemy : MonoBehaviour
{
    [SerializeField] ChessBoardBehavoir boardBehavoir;
    [SerializeField] PieceColor _Color;


    // enemy pieces have positive values
    // player pieces have negative values
    public static int playerPawnValue = -10;
    public static int playerKnightValue = -30;
    public static int playerRookValue = -50;
    public static int playerBishopValue = -30;
    public static int playerQueenValue = -90;
    public static int playerKingValue = -900;
    public static int enemyPawnValue = 10;
    public static int enemyKnightValue = 30;
    public static int enemyRookValue = 50;
    public static int enemyBishopValue = 30;
    public static int enemyQueenValue = 90;
    public static int enemyKingValue = 900;

    bool _Surrender = false;

    public PieceColor Color {get{return _Color;}}
    public bool Surrender {get {return _Surrender;}}


    // set enemy data and delete dragdrop script component from enemy pieces
    public void SetEnemyPieces(PieceColor color) {
        _Color = color;
        ChessPieceBehavoir[] Pieces = boardBehavoir.getPiecesOfColor(_Color);
        for(int i = 0; i < Pieces.Length; i++) {
            if(Pieces[i].GetComponent<ChessPieceBehavoir>().Color == color) {
                Destroy(Pieces[i].GetComponent<DragDrop>());
            }
        }

    }


// Method to calculate move ramdomly, or by depth using minmax
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void moveRandom() {
        ChessPieceBehavoir[] Pieces = boardBehavoir.getPiecesOfColor(_Color);

        ChessBoardPosition boardPositionTree = new ChessBoardPosition();

        // create boardPositions for all pieces
        for(int i = 0; i < Pieces.Length; i++) {
            switch(Pieces[i].Type) {
                case PieceType.King: kingPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Queen: queenPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Bishop: bishopPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Rook: rookPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Knight: knightPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.EnemyPawn: enemyPawnPossibleMoves(boardPositionTree, Pieces[i]); break;
            }
        }

        // move only if there is any possible move
        if(boardPositionTree.getChildrenBoardPositions.Length > 0) {
            //Debug.Log("nUMBER OF CHILDREN: " + boardPositionTree.getChildrenBoardPositions.Length);

            //ramdom position
            int randomChildren = (int)Random.Range(0f,  boardPositionTree.getChildrenBoardPositions.Length);
            
            // get piece and get new column and row
            ChessBoardPositionPieceData pieceToMoveData = boardPositionTree.getChildrenBoardPositions[randomChildren].pieceMoved;
            int actualColumn = boardPositionTree.getChildrenBoardPositions[randomChildren].OldColumn;
            int actualRow = boardPositionTree.getChildrenBoardPositions[randomChildren].OldColumn;
            
            // get reference to piece
            ChessPieceBehavoir pieceToMove = null;
            for(int i = 0; i < Pieces.Length; i++) {
                if(actualColumn == Pieces[i].Column && actualRow == Pieces[i].Row) {pieceToMove = Pieces[i];}
            }

            // move piece to new position
            pieceToMove.movePiece(pieceToMoveData.Column, pieceToMoveData.Row);
            pieceToMove.gameObject.transform.position = boardBehavoir.SquareCenterPosAt(pieceToMoveData.Column, pieceToMoveData.Row);
        }
    }


    public void moveMinMax(int depth, int ramdomly = 0) {
        // get both enemy pieces and player pieces
        ChessPieceBehavoir[] Pieces = boardBehavoir.getPiecesOfColor(_Color);
        ChessBoardPosition boardPositionTree = new ChessBoardPosition(boardBehavoir, Pieces[0]);


        // create boardPositions for all pieces
        for(int i = 0; i < Pieces.Length; i++) {
            switch(Pieces[i].Type) {
                case PieceType.King: kingPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Queen: queenPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Bishop: bishopPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Rook: rookPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.Knight: knightPossibleMoves(boardPositionTree, Pieces[i]); break;
                case PieceType.EnemyPawn: enemyPawnPossibleMoves(boardPositionTree, Pieces[i]); break;
            }
        }

        // move only if there is any possible move
        if(boardPositionTree.getChildrenBoardPositions.Length > 0) {
            // if depth > 1 create tree of possible positions
            int bestValueIndex = 0;


            if(depth == 0) {
                int bestRandomChildren = (int)Random.Range(0f,  boardPositionTree.getChildrenBoardPositions.Length);
            
                // get piece and get new column and row
                ChessBoardPositionPieceData pieceToMoveData = boardPositionTree.getChildrenBoardPositions[bestRandomChildren].pieceMoved;
                int actualColumn = boardPositionTree.getChildrenBoardPositions[bestRandomChildren].OldColumn;
                int actualRow = boardPositionTree.getChildrenBoardPositions[bestRandomChildren].OldRow;

                // get reference to piece
                ChessPieceBehavoir pieceToMove = null;
                for(int i = 0; i < Pieces.Length; i++) {
                    if(actualColumn == Pieces[i].Column && actualRow == Pieces[i].Row) {pieceToMove = Pieces[i];}
                }

                // move piece to new position
                pieceToMove.movePiece(pieceToMoveData.Column, pieceToMoveData.Row);
                pieceToMove.gameObject.transform.position = boardBehavoir.SquareCenterPosAt(pieceToMoveData.Column, pieceToMoveData.Row);
            }
            else if(depth >= 1) {
                // calculate tree from already calculated board positions
                for(int i = 0; i < boardPositionTree.getChildrenBoardPositions.Length; i++) {
                    depthSearchMinMax(boardPositionTree.getChildrenBoardPositions[i], depth);
                }

                // from boardtree stores all index with best value
                // choose ramdom index
                //select best boardPostions and select one ramdomly
                int bestValue = boardPositionTree.getChildrenBoardPositions[bestValueIndex].ValueMinMax;
                ChessBoardPosition bestBoardPositions = new ChessBoardPosition();

                for(int i = 0; i < boardPositionTree.getChildrenBoardPositions.Length; i++) {
                    if(boardPositionTree.getChildrenBoardPositions[i].ValueMinMax == bestValue) {
                        bestBoardPositions.addBoarPosition(boardPositionTree.getChildrenBoardPositions[i]);
                    }
                }
                int bestRandomChildren = (int)Random.Range(0f,  bestBoardPositions.getChildrenBoardPositions.Length);
            
                // get piece and get new column and row
                ChessBoardPositionPieceData pieceToMoveData = bestBoardPositions.getChildrenBoardPositions[bestRandomChildren].pieceMoved;
                int actualColumn = bestBoardPositions.getChildrenBoardPositions[bestRandomChildren].OldColumn;
                int actualRow = bestBoardPositions.getChildrenBoardPositions[bestRandomChildren].OldRow;

                // get reference to piece
                ChessPieceBehavoir pieceToMove = null;
                for(int i = 0; i < Pieces.Length; i++) {
                    if(actualColumn == Pieces[i].Column && actualRow == Pieces[i].Row) {pieceToMove = Pieces[i];}
                }

                // move piece to new position
                pieceToMove.movePiece(pieceToMoveData.Column, pieceToMoveData.Row);
                pieceToMove.gameObject.transform.position = boardBehavoir.SquareCenterPosAt(pieceToMoveData.Column, pieceToMoveData.Row);
            }
        }
    }




    // search given a depth
    void depthSearchMinMax(ChessBoardPosition boardPositionBranch, int depth) {
        //Debug.Log(depth);
        // if not final depth search, once deph is searche, set best value
        if(depth > 1) {
            ChessBoardPositionPieceData[] Pieces = null;


            // select pieces to move depending on player or enemy turn
            // if even then is player turn, min
            // if odd, enemy turn, max
            if(depth % 2 != 0) {
                Pieces = boardPositionBranch.Board.getPiecesOfColor(_Color);
            }
            // select player pieces to move
            else {
                if(_Color == PieceColor.Black) {Pieces = boardPositionBranch.Board.getPiecesOfColor(PieceColor.White);}
                else {Pieces = boardPositionBranch.Board.getPiecesOfColor(PieceColor.Black);}
            }

            

            // create boardPositions for all pieces
            for(int i = 0; i < Pieces.Length; i++) {
                switch(Pieces[i].Type) {
                    case PieceType.King: kingPossibleMoves(boardPositionBranch, Pieces[i]); break;
                    case PieceType.Queen: queenPossibleMoves(boardPositionBranch, Pieces[i]); break;
                    case PieceType.Bishop: bishopPossibleMoves(boardPositionBranch, Pieces[i]); break;
                    case PieceType.Rook: rookPossibleMoves(boardPositionBranch, Pieces[i]); break;
                    case PieceType.Knight: knightPossibleMoves(boardPositionBranch, Pieces[i]); break;
                    case PieceType.EnemyPawn: enemyPawnPossibleMoves(boardPositionBranch, Pieces[i]); break;
                    case PieceType.Pawn: pawnPossibleMoves(boardPositionBranch, Pieces[i]); break;
                }
            }
            //create childs and search them
            if(boardPositionBranch.getChildrenBoardPositions.Length > 0) {
                for(int i = 0; i < boardPositionBranch.getChildrenBoardPositions.Length; i++) {
                    depthSearchMinMax(boardPositionBranch.getChildrenBoardPositions[i], depth - 1);
                }

                //set value depending if player or enemy turn
                // if even then is player turn, min
                // if odd, enemy turn, max
                //choose ramdoom one from the best
                int bestValueIndex = 0;
                if(depth % 2 != 0) {
                    for(int i = 0; i < boardPositionBranch.getChildrenBoardPositions.Length; i++) {
                        if(boardPositionBranch.getChildrenBoardPositions[i].ValueMinMax > boardPositionBranch.getChildrenBoardPositions[bestValueIndex].ValueMinMax) {bestValueIndex = i;}
                    }
                }
                else {
                    for(int i = 0; i < boardPositionBranch.getChildrenBoardPositions.Length; i++) {
                        if(boardPositionBranch.getChildrenBoardPositions[i].ValueMinMax < boardPositionBranch.getChildrenBoardPositions[bestValueIndex].ValueMinMax) {bestValueIndex = i;}
                    }
                }
                boardPositionBranch.ValueMinMax = boardPositionBranch.getChildrenBoardPositions[bestValueIndex].ValueMinMax;         
            }
            //if it has no children positions calculate actual value
            else {calculateBoardValue(boardPositionBranch);
                }
        }
        // if last depth, calculate values
        else {calculateBoardValue(boardPositionBranch);}
    }



    void calculateBoardValue(ChessBoardPosition boardPosition) {
        //ChessBoardPositionPieceData[] WhitePieces = boardPosition.Board.getPiecesOfColor(PieceColor.White);
        //ChessBoardPositionPieceData[] BlackPieces = boardPosition.Board.getPiecesOfColor(PieceColor.Black);

        // check enemy colors
        if(_Color == PieceColor.Black) {
            for(int i = 0; i < boardPosition.Board.gridSize; i++) { 
                for(int j = 0; j < boardPosition.Board.gridSize; j++) {
                    if(boardPosition.Board.pieceAtpos(i,j)) {
                        ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(i,j);
                        if(piece.Color == PieceColor.Black) {
                            switch(piece.Type) {
                                case PieceType.King: boardPosition.ValueMinMax += enemyKingValue; break;
                                case PieceType.Queen: boardPosition.ValueMinMax += enemyQueenValue; break;
                                case PieceType.Bishop: boardPosition.ValueMinMax += enemyBishopValue;break;
                                case PieceType.Rook: boardPosition.ValueMinMax += enemyRookValue; break;
                                case PieceType.Knight: boardPosition.ValueMinMax += enemyKnightValue; break;
                                case PieceType.EnemyPawn: boardPosition.ValueMinMax += enemyPawnValue; break;
                            }
                        }
                        else {
                            switch(piece.Type) {
                                case PieceType.King: boardPosition.ValueMinMax += playerKingValue; break;
                                case PieceType.Queen: boardPosition.ValueMinMax += playerQueenValue; break;
                                case PieceType.Bishop: boardPosition.ValueMinMax += playerBishopValue;break;
                                case PieceType.Rook: boardPosition.ValueMinMax += playerRookValue; break;
                                case PieceType.Knight: boardPosition.ValueMinMax += playerKnightValue; break;
                                case PieceType.Pawn: boardPosition.ValueMinMax += playerPawnValue; break;
                            }
                        }
                    }
                }
            }
        }
        // if enemy color is white
        else{
            for(int i = 0; i < boardPosition.Board.gridSize; i++) { 
                for(int j = 0; j < boardPosition.Board.gridSize; j++) {
                    if(boardPosition.Board.pieceAtpos(i,j)) {
                        ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(i,j);
                        if(piece.Color == PieceColor.White) {
                            switch(piece.Type) {
                                case PieceType.King: boardPosition.ValueMinMax += playerKingValue; break;
                                case PieceType.Queen: boardPosition.ValueMinMax += playerQueenValue; break;
                                case PieceType.Bishop: boardPosition.ValueMinMax += playerBishopValue;break;
                                case PieceType.Rook: boardPosition.ValueMinMax += playerRookValue; break;
                                case PieceType.Knight: boardPosition.ValueMinMax += playerKnightValue; break;
                                case PieceType.EnemyPawn: boardPosition.ValueMinMax += playerPawnValue; break;
                            }
                        }
                        else {
                            switch(piece.Type) {
                                case PieceType.King: boardPosition.ValueMinMax += enemyKingValue; break;
                                case PieceType.Queen: boardPosition.ValueMinMax += enemyQueenValue; break;
                                case PieceType.Bishop: boardPosition.ValueMinMax += enemyBishopValue;break;
                                case PieceType.Rook: boardPosition.ValueMinMax += enemyRookValue; break;
                                case PieceType.Knight: boardPosition.ValueMinMax += enemyKnightValue; break;
                                case PieceType.Pawn: boardPosition.ValueMinMax += enemyPawnValue; break;
                            }
                        }
                    }
                }
            }
        }
    }

    




// Methods to calculate chessboard next position
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






    //method to add new possible movements
    void kingPossibleMoves(ChessBoardPosition boardPositionBranch, ChessPieceBehavoir king) {
        if(king.checkValidMoveFull(king.Column, king.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column, king.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(king.checkValidMoveFull(king.Column + 1, king.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column + 1, king.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column + 1, king.Row)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column + 1, king.Row);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column + 1, king.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column + 1, king.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column, king.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column, king.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column - 1, king.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column - 1, king.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column - 1, king.Row)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column - 1, king.Row);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column - 1, king.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column - 1, king.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
    }
    
    void queenPossibleMoves(ChessBoardPosition boardPositionBranch, ChessPieceBehavoir queen) {
        rookPossibleMoves(boardPositionBranch, queen);
        bishopPossibleMoves(boardPositionBranch, queen);
    }

    void bishopPossibleMoves(ChessBoardPosition boardPositionBranch, ChessPieceBehavoir rook) {
        //check upRight, downRight, upLeft, downLeft diagonals
        int row = rook.Row;
        int column = rook.Column;

        while(column < boardBehavoir.gridSize && row < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column++; 
            row++;
        }

        row = rook.Row;
        column = rook.Column;
        while(column < boardBehavoir.gridSize && row >= 0) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column++; 
            row--;
        }

        row = rook.Row;
        column = rook.Column;
        while(column >= 0 && row < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column--; 
            row++;
        }

        row = rook.Row;
        column = rook.Column;
        while(column >= 0 && row >= 0) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column--; 
            row--;
        }
    }

    void rookPossibleMoves(ChessBoardPosition boardPositionBranch, ChessPieceBehavoir rook) {
        //check up, down, right, left
        int row = rook.Row;
        while(row < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(rook.Column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(rook.Column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            row++;
        }
        
        row = rook.Row;
        while(row >= 0) {
            if(rook.checkValidMoveFull(rook.Column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(rook.Column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            row--;
        }

        int column = rook.Column;
        while(column < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(column, rook.Row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, rook.Row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column++;
        }
        
        column = rook.Column;
        while(column >= 0) {
            if(rook.checkValidMoveFull(column, rook.Row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, rook.Row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column--;
        }
    }

    void knightPossibleMoves(ChessBoardPosition boardPositionBranch, ChessPieceBehavoir knight) {
        if(knight.checkValidMoveFull(knight.Column + 1, knight.Row + 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 1, knight.Row + 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column + 2, knight.Row + 1)) {ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 2, knight.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column + 2, knight.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 2, knight.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column + 1, knight.Row - 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 1, knight.Row - 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 1, knight.Row - 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 1, knight.Row - 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 2, knight.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 2, knight.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 2, knight.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 2, knight.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 1, knight.Row + 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 1, knight.Row + 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }       
    }

    void enemyPawnPossibleMoves(ChessBoardPosition boardPositionBranch, ChessPieceBehavoir pawn) {
        if(pawn.checkValidMoveFull(pawn.Column, pawn.Row - 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column, pawn.Row - 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column, pawn.Row - 1)) {
            
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column, pawn.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column + 1, pawn.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column + 1, pawn.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column - 1, pawn.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardBehavoir, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column - 1, pawn.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //method to add new possible movements
    void kingPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData king) {
        if(king.checkValidMoveFull(king.Column, king.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column, king.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(king.checkValidMoveFull(king.Column + 1, king.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column + 1, king.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column + 1, king.Row)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column + 1, king.Row);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column + 1, king.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column + 1, king.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column, king.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column, king.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column - 1, king.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column - 1, king.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column - 1, king.Row)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column - 1, king.Row);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        if(king.checkValidMoveFull(king.Column - 1, king.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, king);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(king.Column, king.Row);
            piece.movePiece(king.Column - 1, king.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
    }
    
    void queenPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData queen) {
        rookPossibleMoves(boardPositionBranch, queen);
        bishopPossibleMoves(boardPositionBranch, queen);
    }

    void bishopPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData rook) {
        //check upRight, downRight, upLeft, downLeft diagonals
        int row = rook.Row;
        int column = rook.Column;

        while(column < boardBehavoir.gridSize && row < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column++; 
            row++;
        }

        row = rook.Row;
        column = rook.Column;
        while(column < boardBehavoir.gridSize && row >= 0) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column++; 
            row--;
        }

        row = rook.Row;
        column = rook.Column;
        while(column >= 0 && row < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column--; 
            row++;
        }

        row = rook.Row;
        column = rook.Column;
        while(column >= 0 && row >= 0) {
            if(rook.checkValidMoveFull(column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column--; 
            row--;
        }
    }

    void rookPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData rook) {
        //check up, down, right, left
        int row = rook.Row;
        while(row < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(rook.Column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(rook.Column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            row++;
        }
        
        row = rook.Row;
        while(row >= 0) {
            if(rook.checkValidMoveFull(rook.Column, row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(rook.Column, row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            row--;
        }

        int column = rook.Column;
        while(column < boardBehavoir.gridSize) {
            if(rook.checkValidMoveFull(column, rook.Row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, rook.Row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column++;
        }
        
        column = rook.Column;
        while(column >= 0) {
            if(rook.checkValidMoveFull(column, rook.Row)) {
                ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, rook);
                ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(rook.Column, rook.Row);
                piece.movePiece(column, rook.Row);
                boardPosition.pieceMoved = piece;
                boardPositionBranch.addBoarPosition(boardPosition);
            }
            column--;
        }
    }

    void knightPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData knight) {
        //Debug.Log("knight OF COLOR " + knight.Color);
        if(knight.checkValidMoveFull(knight.Column + 1, knight.Row + 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 1, knight.Row + 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column + 2, knight.Row + 1)) {ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 2, knight.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column + 2, knight.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 2, knight.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column + 1, knight.Row - 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column + 1, knight.Row - 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 1, knight.Row - 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 1, knight.Row - 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 2, knight.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 2, knight.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 2, knight.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 2, knight.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(knight.checkValidMoveFull(knight.Column - 1, knight.Row + 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, knight);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(knight.Column, knight.Row);
            piece.movePiece(knight.Column - 1, knight.Row + 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }       
    }

    void enemyPawnPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData pawn) {
        if(pawn.checkValidMoveFull(pawn.Column, pawn.Row - 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column, pawn.Row - 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column, pawn.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column, pawn.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column + 1, pawn.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column + 1, pawn.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column - 1, pawn.Row - 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column - 1, pawn.Row - 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
    }

    void pawnPossibleMoves(ChessBoardPosition boardPositionBranch, ChessBoardPositionPieceData pawn) {
        if(pawn.checkValidMoveFull(pawn.Column, pawn.Row + 2)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column, pawn.Row + 2);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column, pawn.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column, pawn.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column + 1, pawn.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column + 1, pawn.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
        
        if(pawn.checkValidMoveFull(pawn.Column - 1, pawn.Row + 1)) {
            ChessBoardPosition boardPosition = new ChessBoardPosition(boardPositionBranch.Board, pawn);
            ChessBoardPositionPieceData piece = boardPosition.Board.getPieceAtPos(pawn.Column, pawn.Row);
            piece.movePiece(pawn.Column - 1, pawn.Row + 1);
            boardPosition.pieceMoved = piece;
            boardPositionBranch.addBoarPosition(boardPosition);
        }
    }
}