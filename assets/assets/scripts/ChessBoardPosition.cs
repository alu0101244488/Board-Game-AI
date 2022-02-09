using System.Collections;
using System.Collections.Generic;



class ChessBoardPosition {

    //info about the piece moved
    public ChessBoardPositionPieceData pieceMoved;
    int _OldColumn, _OldRow;
    int _BoardValueMinMax = 0;

    // boar data and children board positions
    ChessBoardPositionBoardData boardData;
    ChessBoardPosition[] childrenBoardPositions = new ChessBoardPosition[0];

    public int ValueMinMax {
        get {return _BoardValueMinMax;}
        set {_BoardValueMinMax = value;}
    }

    public int OldColumn {get {return _OldColumn;}}
    public int OldRow {get {return _OldRow;}}

    public ChessBoardPositionBoardData Board {get {return boardData;}}
    public ChessBoardPosition[] getChildrenBoardPositions {get{return childrenBoardPositions;}}



    //add new ChessBoardPosition to childboardpositions
    public ChessBoardPosition() {}

    public ChessBoardPosition(ChessBoardBehavoir boardBehavoir, ChessPieceBehavoir pieceBehavoir) {
        copyChessBoardBehavoir(boardBehavoir);
        setLastPieceMoved(pieceBehavoir);
        _OldColumn = pieceBehavoir.Column;
        _OldRow = pieceBehavoir.Row;
        //reference to piece in boarddata
        //move piece to new position
    }

    public ChessBoardPosition(ChessBoardPositionBoardData boardBehavoir, ChessBoardPositionPieceData pieceBehavoir) {
        copyChessBoardBehavoir(boardBehavoir);
        setLastPieceMoved(pieceBehavoir);
        _OldColumn = pieceBehavoir.Column;
        _OldRow = pieceBehavoir.Row;
        //reference to piece in boarddata
        //move piece to new position
    }

    public void setLastPieceMoved(ChessPieceBehavoir lastPieceMoved) {
        pieceMoved = new ChessBoardPositionPieceData(boardData, lastPieceMoved.Column, lastPieceMoved.Row, lastPieceMoved.Color, lastPieceMoved.Type);
        _OldColumn = lastPieceMoved.Column;
        _OldRow = lastPieceMoved.Row;
    }

    public void setLastPieceMoved(ChessBoardPositionPieceData lastPieceMoved) {
        pieceMoved = new ChessBoardPositionPieceData(boardData, lastPieceMoved.Column, lastPieceMoved.Row, lastPieceMoved.Color, lastPieceMoved.Type);
        _OldColumn = lastPieceMoved.Column;
        _OldRow = lastPieceMoved.Row;
    }

    public void addBoarPosition(ChessBoardPosition childboarPosition) {
        ChessBoardPosition[] newChildrenBoardPositions = new ChessBoardPosition[childrenBoardPositions.Length + 1];
        for(int i = 0; i < childrenBoardPositions.Length; i++) {newChildrenBoardPositions[i] = childrenBoardPositions[i]; }
        newChildrenBoardPositions[newChildrenBoardPositions.Length - 1] = childboarPosition;
        childrenBoardPositions = newChildrenBoardPositions;
    }


    // copy pieces from the real chessboard
    public void copyChessBoardBehavoir(ChessBoardBehavoir copiedBoardBehavoir) {
        boardData = new ChessBoardPositionBoardData();
        boardData.CreateEmptyBoard(); 

        // copy all pieces
        for(int i = 0; i < copiedBoardBehavoir.gridSize; i++) {
            for(int j = 0; j < copiedBoardBehavoir.gridSize; j++) {
                if(copiedBoardBehavoir.pieceAtpos(i, j)) {
                    ChessPieceBehavoir piece = copiedBoardBehavoir.getPieceAtPos(i, j);
                    boardData.addPieceAtPos(piece.Column, piece.Row, piece.Color, piece.Type);
                    boardData.updateBoardAtPos(piece.Column, piece.Row);
                   
                    //if piece already moved, set to true
                    if(piece.AlreadyMoved) {boardData.getPieceAtPos(piece.Column, piece.Row).AlreadyMoved = true;}
                }
            }
        }
    }

    public void copyChessBoardBehavoir(ChessBoardPositionBoardData copiedBoardBehavoir) {
        boardData = new ChessBoardPositionBoardData();
        boardData.CreateEmptyBoard(); 

        // copy all pieces
        for(int i = 0; i < copiedBoardBehavoir.gridSize; i++) {
            for(int j = 0; j < copiedBoardBehavoir.gridSize; j++) {
                if(copiedBoardBehavoir.pieceAtpos(i, j)) {
                    ChessBoardPositionPieceData piece = copiedBoardBehavoir.getPieceAtPos(i, j);
                    boardData.addPieceAtPos(piece.Column, piece.Row, piece.Color, piece.Type);
                    boardData.updateBoardAtPos(piece.Column, piece.Row);
                   
                    //if piece already moved, set to true
                    if(piece.AlreadyMoved) {boardData.getPieceAtPos(piece.Column, piece.Row).AlreadyMoved = true;}
                }
            }
        }
    }
}