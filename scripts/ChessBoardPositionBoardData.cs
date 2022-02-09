using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ChessBoardPositionBoardData
{
    //public int sideSquareCount
    public int gridSize = 8;   
    
    // data of the pieces in the board: array of pieces, ocupied squares in the board
    ChessBoardPositionPieceData[] Pieces = new ChessBoardPositionPieceData[0];
    bool[][] pieceInSquare;



    // return null if not piece, return piece in other case
    public bool pieceAtpos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            return pieceInSquare[column][row];
        }
        return false;
    }

    // return rteference of the pieces info
    public ChessBoardPositionPieceData getPieceAtPos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            for(int i = 0; i < Pieces.Length; i++) {
                ChessBoardPositionPieceData pieceBehavoir = Pieces[i];
                if(pieceBehavoir.Column == column && pieceBehavoir.Row == row) {return pieceBehavoir;}
            }
        }
        return null;
    }

    // return array of all pieces of a given color
    public ChessBoardPositionPieceData[] getPiecesOfColor(PieceColor color) {
        ChessBoardPositionPieceData[] ColorPieces = new ChessBoardPositionPieceData[0];
        ChessBoardPositionPieceData[] copyColorPieces = new ChessBoardPositionPieceData[0];

        for(int i = 0; i < Pieces.Length; i++) {
            ChessBoardPositionPieceData piece = Pieces[i];
            if(piece.Color == color) {
                copyColorPieces = new ChessBoardPositionPieceData[ColorPieces.Length + 1];
                for(int j = 0; j < ColorPieces.Length; j++) {
                    copyColorPieces[j] = ColorPieces[j];
                }
                copyColorPieces[copyColorPieces.Length - 1] = piece;
                ColorPieces = copyColorPieces;
            }
        }
        return copyColorPieces;
    }
    
    // methods to modify the arrya of pieces ofthe board (add or remove)
    public void addPieceAtPos(int column, int row, PieceColor color, PieceType type) {
        if(!pieceAtpos(column, row)) {
            ChessBoardPositionPieceData[] newPiecesArray = new ChessBoardPositionPieceData[Pieces.Length + 1];
            for(int i = 0; i < Pieces.Length; i++) {
                newPiecesArray[i] = Pieces[i];
            }
            newPiecesArray[newPiecesArray.Length - 1] = CreateChessPiece(column, row, color, type);
            Pieces = newPiecesArray;
        }
        
    }

    public void deletePieceAtPos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            ChessBoardPositionPieceData[] newPiecesArray;
            for(int i = 0; i < Pieces.Length; i++) {
                ChessBoardPositionPieceData piece = Pieces[i];
                if(piece.Column == column && piece.Row == row) {
                    newPiecesArray = new ChessBoardPositionPieceData[Pieces.Length - 1];
                    Pieces[i] = Pieces[Pieces.Length - 1];
                    for(int j = 0; j < newPiecesArray.Length; j++) {
                        newPiecesArray[j] = Pieces[j];
                    }
                    Pieces = newPiecesArray;
                }
            }
        }
    }

    public void updateBoardAtPos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            pieceInSquare[column][row] = !pieceInSquare[column][row];
        }
    }



    // set all values of the board
    public void CreateEmptyBoard() {
        // set all positions to be empty
        pieceInSquare = new bool[gridSize][];
        for(int i = 0; i < gridSize; i++) {
            pieceInSquare[i] = new bool[gridSize];
            for(int j = 0; j < gridSize; j++) {pieceInSquare[i][j] = false;}
        }
    }



    // method to create pieces
    public ChessBoardPositionPieceData CreateChessPiece(int column, int row, PieceColor color, PieceType type) {  
        ChessBoardPositionPieceData piece = new ChessBoardPositionPieceData();
        piece.SetPieceData(this, column, row, color, type);
        return piece;
    }
}


