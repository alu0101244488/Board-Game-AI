using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[HideInInspector] public enum PieceType {King, Queen, Bishop, Rook, Knight, Pawn, EnemyPawn}
[HideInInspector] public enum PieceColor {Black, White}
//[HideInInspector] enum GameType {Normal, captureAllPieces}

public class ChessBoardBehavoir : MonoBehaviour
{
    //public int sideSquareCount
    public int gridSize = 8;
    
    // variables to modify the transform of pieces to align them with the board squares
    public float pieceOffsetX = -2.305f;
    public float pieceOffsetY = -2.305f;
    public float pieceSpacing = 0.66f;
    public float InfrontPosZ = -1;
    static Vector3[][] squareCenters;
    public float squareCenterRange = 0.1f;

    // reference to the chess piece prefab
    [SerializeField] GameObject ChessPiece;
    
    // sprites of all chess pieces
    [SerializeField] Sprite black_king, black_queen, black_bishop, black_knight, black_rook, black_pawn;
    [SerializeField] Sprite  white_king, white_queen, white_bishop, white_knight, white_rook, white_pawn;
    
    // data of the pieces in the board: array of pieces, ocupied squares in the board
    static GameObject[] Pieces = new GameObject[0];
    static bool[][] pieceInSquare;

    // centers of squares in the board to place the pieces
    public Vector3 SquareCenterPosAt(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {return squareCenters[column][row];}
        return Vector3.zero;
    }

    // return null if not piece, return piece in other case
    public bool pieceAtpos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            return pieceInSquare[column][row];
        }
        return false;
    }

    // return rteference of the pieces info
    public ChessPieceBehavoir getPieceAtPos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            for(int i = 0; i < Pieces.Length; i++) {
                ChessPieceBehavoir pieceBehavoir = Pieces[i].GetComponent<ChessPieceBehavoir>();
                if(pieceBehavoir.Column == column && pieceBehavoir.Row == row) {return pieceBehavoir;}
            }
        }
        return null;
    }
/*
    // return array of all pieces of a given color
    public ChessPieceBehavoir[] getWhitePieces() {

        ChessPieceBehavoir[] WhitePieces = new ChessPieceBehavoir[0];
        ChessPieceBehavoir[] copyWhitePieces  = new ChessPieceBehavoir[0];

        for(int i = 0; i < Pieces.Length; i++) {
            ChessPieceBehavoir pieceBehavoir = Pieces[i].GetComponent<ChessPieceBehavoir>();
            if(pieceBehavoir.Color == PieceColor.White) {
                copyWhitePieces = new ChessPieceBehavoir[WhitePieces.Length + 1];
                for(int j = 0; j < WhitePieces.Length; j++) {
                    copyWhitePieces[j] = WhitePieces[j];
                }
                copyWhitePieces[copyWhitePieces.Length - 1] = pieceBehavoir;
                WhitePieces = copyWhitePieces;
            }
        }
        return copyWhitePieces;
    }

    public ChessPieceBehavoir[] getBlackPieces() {

        ChessPieceBehavoir[] BlackPieces = new ChessPieceBehavoir[0];
        ChessPieceBehavoir[] copyBlackPieces = new ChessPieceBehavoir[0];

        for(int i = 0; i < Pieces.Length; i++) {
            ChessPieceBehavoir pieceBehavoir = Pieces[i].GetComponent<ChessPieceBehavoir>();
            if(pieceBehavoir.Color == PieceColor.Black) {
                copyBlackPieces = new ChessPieceBehavoir[BlackPieces.Length + 1];
                for(int j = 0; j < BlackPieces.Length; j++) {
                    copyBlackPieces[j] = BlackPieces[j];
                }
                copyBlackPieces[copyBlackPieces.Length - 1] = pieceBehavoir;
                BlackPieces = copyBlackPieces;
            }
        }
        return copyBlackPieces;
    }
*/
    public ChessPieceBehavoir[] getPiecesOfColor(PieceColor color) {
        ChessPieceBehavoir[] ColorPieces = new ChessPieceBehavoir[0];
        ChessPieceBehavoir[] copyColorPieces = new ChessPieceBehavoir[0];

        for(int i = 0; i < Pieces.Length; i++) {
            ChessPieceBehavoir pieceBehavoir = Pieces[i].GetComponent<ChessPieceBehavoir>();
            if(pieceBehavoir.Color == color) {
                copyColorPieces = new ChessPieceBehavoir[ColorPieces.Length + 1];
                for(int j = 0; j < ColorPieces.Length; j++) {
                    copyColorPieces[j] = ColorPieces[j];
                }
                copyColorPieces[copyColorPieces.Length - 1] = pieceBehavoir;
                ColorPieces = copyColorPieces;
            }
        }
        return copyColorPieces;
    }
    
    // methods to modify the arrya of pieces ofthe board (add or remove)
    public void addPieceAtPos(int column, int row, PieceColor color, PieceType type) {
        if(!pieceAtpos(column, row)) {
            GameObject[] newPiecesArray = new GameObject[Pieces.Length + 1];
            for(int i = 0; i < Pieces.Length; i++) {
                newPiecesArray[i] = Pieces[i];
            }
            newPiecesArray[newPiecesArray.Length - 1] = CreateChessPiece(this, column, row, color, type);
            Pieces = newPiecesArray;
        }
        
    }

    public void deletePieceAtPos(int column, int row) {
        if(column >= 0 && column < gridSize && row >= 0 && row < gridSize) {
            GameObject[] newPiecesArray;
            for(int i = 0; i < Pieces.Length; i++) {
                ChessPieceBehavoir pieceBehavoir = Pieces[i].GetComponent<ChessPieceBehavoir>();
                if(pieceBehavoir.Column == column && pieceBehavoir.Row == row) {
                    newPiecesArray = new GameObject[Pieces.Length - 1];
                    Destroy(Pieces[i]);
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


    public void destroyPieces() {
        for(int i = 0; i < Pieces.Length; i++) {
            Destroy(Pieces[i].gameObject);
        }
    }

    // set all values of the board
    public void CreateEmptyBoard() {
        // store center point of every square in the board
        float squareCenterX, squareCenterY;
        squareCenters = new Vector3[gridSize][];

        for(int i = 0; i < gridSize; i++) {
            squareCenters[i] = new Vector3[gridSize];
            squareCenterX = i * pieceSpacing +  pieceOffsetX;
            
            for(int j = 0; j < gridSize; j++) {
                squareCenterY = j * pieceSpacing +  pieceOffsetY;
                squareCenters[i][j] = new Vector3(squareCenterX, squareCenterY, InfrontPosZ);
            }
        }

        // set all positions to be empty
        pieceInSquare = new bool[gridSize][];
        for(int i = 0; i < gridSize; i++) {
            pieceInSquare[i] = new bool[gridSize];
            for(int j = 0; j < gridSize; j++) {pieceInSquare[i][j] = false;}
        }
    }

    
    // method to create pieces
   public GameObject CreateChessPiece(ChessBoardBehavoir boardBehavoir, int column, int row, PieceColor color, PieceType type) {
        // modify piece transform and set piece data
        float piecePositionX = column * boardBehavoir.pieceSpacing + boardBehavoir.pieceOffsetX;
        float piecePositionY = row * boardBehavoir.pieceSpacing + boardBehavoir.pieceOffsetY;
        Vector3 piecePosition = new Vector3(piecePositionX, piecePositionY, boardBehavoir.InfrontPosZ);
        GameObject piece = Instantiate(ChessPiece, piecePosition, Quaternion.identity);
        ChessPieceBehavoir pieceData = piece.GetComponent<ChessPieceBehavoir>();
        pieceData.SetPieceData(this, column, row, color, type);

        // apply sprite to chess piece depending on color and type
        switch(color) {
            case PieceColor.White:
                switch(type) {
                    case PieceType.King: piece.GetComponent<SpriteRenderer>().sprite = white_king; break;
                    case PieceType.Queen: piece.GetComponent<SpriteRenderer>().sprite = white_queen; break;
                    case PieceType.Bishop: piece.GetComponent<SpriteRenderer>().sprite = white_bishop; break;
                    case PieceType.Knight: piece.GetComponent<SpriteRenderer>().sprite = white_knight; break;
                    case PieceType.Rook: piece.GetComponent<SpriteRenderer>().sprite = white_rook; break;
                    case PieceType.Pawn: piece.GetComponent<SpriteRenderer>().sprite = white_pawn; break;
                    case PieceType.EnemyPawn: piece.GetComponent<SpriteRenderer>().sprite = white_pawn; break;
                }
                break;
            case PieceColor.Black:
                switch(type) {
                    case PieceType.King: piece.GetComponent<SpriteRenderer>().sprite = black_king; break;
                    case PieceType.Queen: piece.GetComponent<SpriteRenderer>().sprite = black_queen; break;
                    case PieceType.Bishop: piece.GetComponent<SpriteRenderer>().sprite = black_bishop; break;
                    case PieceType.Knight: piece.GetComponent<SpriteRenderer>().sprite = black_knight; break;
                    case PieceType.Rook: piece.GetComponent<SpriteRenderer>().sprite = black_rook; break;
                    case PieceType.Pawn: piece.GetComponent<SpriteRenderer>().sprite = black_pawn; break;
                    case PieceType.EnemyPawn: piece.GetComponent<SpriteRenderer>().sprite = black_pawn; break;
                }
                break;
        }
        return piece;
    }


    // method to create an empty board and then with the fen string
    // generate all the pieces in the board
    public void GenerateBoard(string fenBoardPosition) {
        CreateEmptyBoard();

        // FEN parser, starts from left to right corner on each row (from top row to botton)
        Pieces = new GameObject[0];
        string[] fenRows = fenBoardPosition.Split('/');

        for(int i = 0; i < fenRows.Length; i++) {
            
            int column = 0;

            for(int j = 0; j < fenRows[i].Length; j++) {
                //check if is number or character
                if(char.IsNumber(fenRows[i][j])) {column += fenRows[i][j] - '0' - 1;}
                else {
                    int row = gridSize - i - 1;
                    switch(fenRows[i][j]) {
                        case 'P': addPieceAtPos(column, row, PieceColor.White, PieceType.Pawn); break;
                        case 'N': addPieceAtPos(column, row, PieceColor.White, PieceType.Knight); break;
                        case 'R': addPieceAtPos(column, row, PieceColor.White, PieceType.Rook); break;
                        case 'B': addPieceAtPos(column, row, PieceColor.White, PieceType.Bishop); break;
                        case 'Q': addPieceAtPos(column, row, PieceColor.White, PieceType.Queen); break;
                        case 'K': addPieceAtPos(column, row, PieceColor.White, PieceType.King); break;
                        case 'p': addPieceAtPos(column, row, PieceColor.Black, PieceType.EnemyPawn); break;
                        case 'n': addPieceAtPos(column, row, PieceColor.Black, PieceType.Knight); break;
                        case 'r': addPieceAtPos(column, row, PieceColor.Black, PieceType.Rook); break;
                        case 'b': addPieceAtPos(column, row, PieceColor.Black, PieceType.Bishop); break;
                        case 'q': addPieceAtPos(column, row, PieceColor.Black, PieceType.Queen); break;
                        case 'k': addPieceAtPos(column, row, PieceColor.Black, PieceType.King); break;
                    }
                    pieceInSquare[column][row] = true;
                    column++;
                }
            }
        }
    }
}
