using System.Collections;
using System.Collections.Generic;




public class ChessBoardPositionPieceData
{
    // column = x axis, row = y axis
    int _Column;
    int _Row;
    PieceColor _Color;
    PieceType _Type;
    
    bool _FirstTimeMoved = false;
    ChessBoardPositionBoardData boardData;


    // delegate method that store the movement of the piece and the attack
    public delegate bool PieceMove(int newColumn, int newRow);
    public PieceMove checkValidMove;

    delegate bool PieceAttack(int newColumn, int newRow);
    public PieceMove checkValidAttack;

    public int Column {
        get {return _Column;} 
        set {_Column = value;}
    }
    public int Row {
        get {return _Row;}
        set {_Row = value;}
    }
    public PieceColor Color {get{return _Color;}}
    public PieceType Type {get{return _Type;}}

    public bool AlreadyMoved {
        get{return _FirstTimeMoved;}
        set {_FirstTimeMoved = value;}
    }

    public ChessBoardPositionPieceData() {}
    public ChessBoardPositionPieceData(ChessBoardPositionBoardData chessBoardData, int column, int row, PieceColor color, PieceType type) {
        SetPieceData(chessBoardData, column, row, color, type);
    }
    // set data of the piece
    public void SetPieceData(ChessBoardPositionBoardData chessBoardData, int column, int row, PieceColor color, PieceType type) {
        _Column = column;
        _Row = row;
        _Color = color;
        _Type = type;
        boardData = chessBoardData;

        //set valid move and valid attack methods
        switch(_Type) {
            case PieceType.King: 
                checkValidMove = KingMove;
                checkValidAttack = KingAttack; 
                break;
            case PieceType.Queen: 
                checkValidMove = QueenMove;
                checkValidAttack = QueenAttack; 
                break;
            case PieceType.Bishop: 
                checkValidMove = BishopMove;
                checkValidAttack = BishopAttack;  
                break;
            case PieceType.Rook: 
                checkValidMove = RookMove;
                checkValidAttack = RookAttack;  
                break;
            case PieceType.Knight: 
                checkValidMove = KnightMove;
                checkValidAttack = KnightAttack;  
                break;
            case PieceType.Pawn: 
                checkValidMove = PawnMove;
                checkValidAttack = PawnAttack; 
                break;
            case PieceType.EnemyPawn: 
                checkValidMove = EnemyPawnMove;
                checkValidAttack = EnemyPawnAttack; 
                break;
        }
    }

    // takes into account if king is under attack 
    // to check if the movement is valid or not
    /*
    public bool checkValidMoveFull(int newColumn, int newRow) {
        if(checkValidMove(newColumn, newRow)) {return KingUnderAttack(newColumn, newRow);}
        return false;
    }
    */

    // method to update the board
    public void movePiece(int newColumn, int newRow) {
        boardData.updateBoardAtPos(_Column, _Row);

        if(boardData.pieceAtpos(newColumn, newRow)) {boardData.deletePieceAtPos(newColumn, newRow);}
        else {boardData.updateBoardAtPos(newColumn, newRow);}

        _Column = newColumn;
        _Row = newRow;  

        // return ChessBoardPosition
        // with before the movement      
    }


    public bool checkValidMoveFull(int newColumn, int newRow) {return checkValidMove(newColumn, newRow);}

//Piece moves
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //methods to check if the move is valid
    bool KingMove(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {

            //check if occupied
            if(boardData.pieceAtpos(newColumn, newRow)) {
                if(boardData.getPieceAtPos(newColumn, newRow).Color == _Color) {return false;}
            }            

            //check if king can be attacked from new position
            for(int i = 0; i < boardData.gridSize; i++) {
                for(int j = 0; j < boardData.gridSize; j++) {
                    if(boardData.pieceAtpos(i, j)) {
                        ChessBoardPositionPieceData enemyPiece = boardData.getPieceAtPos(i,j);
                        if(enemyPiece.Color != _Color) {
                            if(enemyPiece.checkValidAttack(newColumn, newRow)) {return false;}
                        }
                    }
                }
            }

            //check if valid movement
            if((_Column == newColumn || _Column + 1 == newColumn || _Column - 1 == newColumn)) {
                if(_Row == newRow || _Row + 1 == newRow || _Row - 1 == newRow) {return true;}
            }
        }
        return false;
    }


    bool QueenMove(int newColumn, int newRow) {
        // check if move is rook like then if is bishop like
        if(RookMove(newColumn, newRow)) {return true;}
        else if(BishopMove(newColumn, newRow)) {return true;}
        return false;
    }



    bool BishopMove(int newColumn, int newRow) {
        if(newColumn != Column && newRow != _Row) {
            // default maximum allowed move on each direction
            // upRight, upLeft, dowonRight, downRight
            int maxUpRightColumn = boardData.gridSize;
            int maxUpRightRow = boardData.gridSize;

            int maxUpLeftColumn = 0;
            int maxUpLeftRow = boardData.gridSize;

            int maxDownRightColumn = boardData.gridSize;
            int maxDownRightRow = 0;

            int maxDownLeftColumn = 0;
            int maxDownLeftRow = 0;

            // search for the nearest piece upRight
            int column = _Column;//column
            int row = _Row;  //row
            while(!boardData.pieceAtpos(column + 1, row + 1) && column + 1 < boardData.gridSize && row + 1 < boardData.gridSize) {
                column++; row++;
            }
            //check if next square column+1 row+1 there is an enemy or outside board
            if(boardData.pieceAtpos(column + 1, row + 1) && column < boardData.gridSize && row < boardData.gridSize) {
                if(boardData.getPieceAtPos(column + 1, row + 1).Color != _Color) {
                    column++; row++;
                }
            }
            maxUpRightColumn = column;
            maxUpRightRow = row;

            // search for the nearest piece upLeft
            column = _Column;
            row = _Row;
            while(!boardData.pieceAtpos(column - 1, row + 1) && column >= 0 && row < boardData.gridSize) {
                column--; row++;
            }
            if(boardData.pieceAtpos(column - 1, row + 1) && column > 0 && row < boardData.gridSize) {
                if(boardData.getPieceAtPos(column - 1, row + 1).Color != _Color) {
                    column--; row++;
                }
            }
            maxUpLeftColumn = column;
            maxUpLeftRow = row;

            // search for the nearest piece downRight
            column = _Column;
            row = _Row; 
            while(!boardData.pieceAtpos(column + 1, row - 1) && column < boardData.gridSize && row >= 0) {
                column++; row--;
            }
            if(boardData.pieceAtpos(column + 1, row - 1) && column < boardData.gridSize && row > 0) {
                if(boardData.getPieceAtPos(column + 1, row - 1).Color != _Color) {
                    column++; row--;
                }
            }
            maxDownRightColumn = column;
            maxDownRightRow = row;

            // search for the nearest piece downLeft
            column = _Column;
            row = _Row; 
            while(!boardData.pieceAtpos(column - 1, row - 1) && column >= 0 && row >= 0) {
                column--; row--;
            }
            //check if next square column+1 row-1 there is an enemy or outside board
            if(boardData.pieceAtpos(column - 1, row - 1) && column > 0 && row > 0) {
                if(boardData.getPieceAtPos(column - 1, row - 1).Color != _Color) {
                    column--; row--;
                }
            }
            maxDownLeftColumn = column;
            maxDownLeftRow = row;

            // check if the new position is valid
            // chek if new position is in upright diagonal
            column = _Column + 1;
            row = _Row + 1;
            while(newColumn != column && newRow != row && column < maxUpRightColumn && row < maxUpRightRow) {
                column++; row++;
            }

            if(newColumn == column && newRow == row && column <= maxUpRightColumn && row <= maxUpRightRow) {return true;}

            // chek if new position is in upLeft diagonal
            column = _Column - 1;
            row = _Row + 1;
            while(newColumn != column && newRow != row && column > maxUpLeftColumn && row < maxUpLeftRow) {
                column--; row++;
            }
            if(newColumn == column && newRow == row && column >= maxUpLeftColumn && row <= maxUpLeftRow) {return true;}

            // chek if new position is in downRight diagonal
            column = _Column + 1;
            row = _Row - 1;

            while(newColumn != column && newRow != row && column < maxDownRightColumn && row > maxDownRightRow) {
                column++; row--;
            }
            if(newColumn == column && newRow == row && column <= maxDownRightColumn && row >= maxDownRightRow) {return true;}

            // chek if new position is in downLeft diagonal
            column = _Column - 1;
            row = _Row - 1;

            while(newColumn != column && newRow != row && column > maxDownLeftColumn && row > maxDownLeftRow) {
                column--; row--;
            }
            if(newColumn == column && newRow == row && column >= maxDownLeftColumn && row >= maxDownLeftRow) {return true;}
        }
        return false;
    }
    


    bool RookMove(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            if((newColumn != Column && newRow == _Row) || (newColumn == Column && newRow != _Row)) {
                // default maximum allowed move on each direction
                int maxUp = boardData.gridSize;
                int maxDown = 0;
                int maxRight = boardData.gridSize;
                int maxLeft = 0;

                // search for the nearest piece to the rook up, down, right and down

                //cambiar a while no haya ficha column++ row
                int column = _Column;
                int row = _Row;
                for(int i = boardData.gridSize - 1; i > _Row; i--) {
                    if(boardData.pieceAtpos(_Column, i)) {
                        if(boardData.getPieceAtPos(_Column, i).Color != _Color) {maxUp = i + 1;}
                        else {maxUp = i;}
                    }
                }
                for(int i = 0; i < _Row; i++) {
                    if(boardData.pieceAtpos(_Column, i)) {
                        if(boardData.getPieceAtPos(_Column, i).Color != _Color) {maxDown = i;}
                        else {maxDown = i + 1;}
                    }
                }
                for(int i = boardData.gridSize - 1; i > _Column; i--) {
                    if(boardData.pieceAtpos(i, _Row)) {
                        if(boardData.getPieceAtPos(i, _Row).Color != _Color) {maxRight = i + 1;}
                        else {maxRight = i;}
                    }
                }
                for(int i = 0; i < _Column; i++) {
                    if(boardData.pieceAtpos(i, _Row)) {
                        if(boardData.getPieceAtPos(i, _Row).Color != _Color) {maxLeft = i;}
                        else {maxLeft = i + 1;}
                    }
                }

                // check if the movement is valid (vertically or horizontally)
                if((newColumn < maxRight && newColumn >= maxLeft && newRow == Row)  || (newRow < maxUp && newRow >= maxDown && newColumn == Column)) {return true;}
                //delete if inlimits from this if
            }
        }
        return false;
    }



    bool KnightMove(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            //check for piece of same color
            if(boardData.pieceAtpos(newColumn, newRow)) {
                if(boardData.getPieceAtPos(newColumn, newRow).Color == _Color) {return false;}
            }

            //check if valid knight move
            if(newColumn == _Column - 1 && newRow == _Row + 2 ) {return true;}
            else if(newColumn == _Column + 1 && newRow == _Row + 2) {return true;}
            else if(newColumn == _Column + 2 && newRow == _Row + 1) {return true;}
            else if(newColumn == _Column + 2 && newRow == _Row - 1) {return true;}
            else if(newColumn == _Column + 1 && newRow == _Row - 2) {return true;}
            else if(newColumn == _Column - 1 && newRow == _Row - 2) {return true;}
            else if(newColumn == _Column - 2 && newRow == _Row - 1) {return true;}
            else if(newColumn == _Column - 2 && newRow == _Row + 1) {return true;}
        }
        return false;
    }



    bool PawnMove(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            // special  first move
            if(!_FirstTimeMoved && _Row + 2 == newRow && _Column == newColumn && !boardData.pieceAtpos(newColumn, _Row + 1)) {
                if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                return true;
            }
            
            //normal movements
            if(_Row + 1  == newRow) {
                //move up, right or left
                if(_Column == newColumn && !boardData.pieceAtpos(newColumn, _Row + 1)) {
                    if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                    return true;
                }

                else if(_Column - 1 == newColumn && boardData.pieceAtpos(newColumn, _Row + 1)) {
                    if(boardData.getPieceAtPos(_Column - 1, Row + 1).Color != _Color) {
                        if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                        return true;
                    }
                }

                else if(_Column + 1 == newColumn && boardData.pieceAtpos(newColumn, _Row + 1)) {
                    if(boardData.getPieceAtPos(_Column + 1, Row + 1).Color != _Color) {
                        if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                        return true;
                    }
                }
            }
        }
        return false;
    }



    // the enemy pawn moves from the top of the board to the  bottom
    bool EnemyPawnMove(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            // special  first move
            if(!_FirstTimeMoved && _Row - 2 == newRow && _Column == newColumn && !boardData.pieceAtpos(newColumn, _Row - 1)) {
                if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                return true;
            }
            
            //normal movements
            if(_Row - 1  == newRow) {
                //move up, right or left
                if(_Column == newColumn && !boardData.pieceAtpos(newColumn, _Row - 1)) {
                    if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                    return true;
                }

                else if(_Column - 1 == newColumn && boardData.pieceAtpos(newColumn, _Row - 1)) {
                    if(boardData.getPieceAtPos(_Column - 1, Row - 1).Color != _Color) {
                        if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                        return true;
                    }
                }

                else if(_Column + 1 == newColumn && boardData.pieceAtpos(newColumn, _Row - 1)) {
                    if(boardData.getPieceAtPos(_Column + 1, Row - 1).Color != _Color) {
                        if(!_FirstTimeMoved) {_FirstTimeMoved = true;}
                        return true;
                    }
                }
            }
        }
        return false;
    }

//Piece attacks
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


   


    //methods to check if the attack is valid (for the unvalid moves of the king)
    bool KingAttack(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            //check if valid movement
            if((_Column == newColumn || _Column + 1 == newColumn || _Column - 1 == newColumn)) {
                if(_Row == newRow || _Row + 1 == newRow || _Row - 1 == newRow) {return true;}
            }
        }
        return false;
    }



    bool QueenAttack(int newColumn, int newRow) {
        // check if move is rook like then if is bishop like
        if(RookAttack(newColumn, newRow)) {return true;}
        else if(BishopAttack(newColumn, newRow)) {return true;}
        return false;
    }



    bool BishopAttack(int newColumn, int newRow) {
        if(newColumn != Column && newRow != _Row) {
            // default maximum allowed move on each direction
            // upRight, upLeft, dowonRight, downRight
            int maxUpRightColumn = boardData.gridSize;
            int maxUpRightRow = boardData.gridSize;

            int maxUpLeftColumn = 0;
            int maxUpLeftRow = boardData.gridSize;

            int maxDownRightColumn = boardData.gridSize;
            int maxDownRightRow = 0;

            int maxDownLeftColumn = 0;
            int maxDownLeftRow = 0;

            // search for the nearest piece upRight
            int column = _Column;//column
            int row = _Row;  //row
            while(!boardData.pieceAtpos(column + 1, row + 1) && column + 1 < boardData.gridSize && row + 1 < boardData.gridSize) {
                column++; row++;
            }
            column++; row++;
            maxUpRightColumn = column;
            maxUpRightRow = row;

            // search for the nearest piece upLeft
            column = _Column;
            row = _Row;
            while(!boardData.pieceAtpos(column - 1, row + 1) && column >= 0 && row < boardData.gridSize) {
                column--; row++;
            }
            column--; row++;
            maxUpLeftColumn = column;
            maxUpLeftRow = row;

            // search for the nearest piece downRight
            column = _Column;
            row = _Row; 
            while(!boardData.pieceAtpos(column + 1, row - 1) && column < boardData.gridSize && row >= 0) {
                column++; row--;
            }
            column++; row--;
            maxDownRightColumn = column;
            maxDownRightRow = row;

            // search for the nearest piece downLeft
            column = _Column;
            row = _Row; 
            while(!boardData.pieceAtpos(column - 1, row - 1) && column >= 0 && row >= 0) {
                column--; row--;
            }
            column--; row--;
            maxDownLeftColumn = column;
            maxDownLeftRow = row;

            // check if the new position is valid
            // chek if new position is in upright diagonal
            column = _Column + 1;
            row = _Row + 1;
            while(newColumn != column && newRow != row && column < maxUpRightColumn && row < maxUpRightRow) {
                column++; row++;
            }

            if(newColumn == column && newRow == row && column <= maxUpRightColumn && row <= maxUpRightRow) {return true;}

            // chek if new position is in upLeft diagonal
            column = _Column - 1;
            row = _Row + 1;
            while(newColumn != column && newRow != row && column > maxUpLeftColumn && row < maxUpLeftRow) {
                column--; row++;
            }
            if(newColumn == column && newRow == row && column >= maxUpLeftColumn && row <= maxUpLeftRow) {return true;}

            // chek if new position is in downRight diagonal
            column = _Column + 1;
            row = _Row - 1;

            while(newColumn != column && newRow != row && column < maxDownRightColumn && row > maxDownRightRow) {
                column++; row--;
            }
            if(newColumn == column && newRow == row && column <= maxDownRightColumn && row >= maxDownRightRow) {return true;}

            // chek if new position is in downLeft diagonal
            column = _Column - 1;
            row = _Row - 1;

            while(newColumn != column && newRow != row && column > maxDownLeftColumn && row > maxDownLeftRow) {
                column--; row--;
            }
            if(newColumn == column && newRow == row && column >= maxDownLeftColumn && row >= maxDownLeftRow) {return true;}
        }
        return false;
    }



    bool RookAttack(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            if((newColumn != Column && newRow == _Row) || (newColumn == Column && newRow != _Row)) {
                // default maximum allowed move on each direction
                int maxUp = boardData.gridSize;
                int maxDown = 0;
                int maxRight = boardData.gridSize;
                int maxLeft = 0;

                // search for the nearest piece to the rook up, down, right and down

                //cambiar a while no haya ficha column++ row
                int column = _Column;
                int row = _Row;
                for(int i = boardData.gridSize - 1; i > _Row; i--) {
                    if(boardData.pieceAtpos(_Column, i)) {maxUp = i + 1;}
                }
                for(int i = 0; i < _Row; i++) {
                    if(boardData.pieceAtpos(_Column, i)) {maxDown = i;}
                }
                for(int i = boardData.gridSize - 1; i > _Column; i--) {
                    if(boardData.pieceAtpos(i, _Row)) {maxRight = i + 1;}
                }
                for(int i = 0; i < _Column; i++) {
                    if(boardData.pieceAtpos(i, _Row)) {maxLeft = i;}
                }

                // check if the movement is valid (vertically or horizontally)
                if((newColumn < maxRight && newColumn >= maxLeft && newRow == Row)  || (newRow < maxUp && newRow >= maxDown && newColumn == Column)) {return true;}
                //delete if inlimits from this if
            }
        }
        return false;
    }



    bool KnightAttack(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            //check if valid knight move
            if(newColumn == _Column - 1 && newRow == _Row + 2 ) {return true;}
            else if(newColumn == _Column + 1 && newRow == _Row + 2) {return true;}
            else if(newColumn == _Column + 2 && newRow == _Row + 1) {return true;}
            else if(newColumn == _Column + 2 && newRow == _Row - 1) {return true;}
            else if(newColumn == _Column + 1 && newRow == _Row - 2) {return true;}
            else if(newColumn == _Column - 1 && newRow == _Row - 2) {return true;}
            else if(newColumn == _Column - 2 && newRow == _Row - 1) {return true;}
            else if(newColumn == _Column - 2 && newRow == _Row + 1) {return true;}
        }
        return false;
    }



    bool PawnAttack(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            //normal movements
            if(_Row + 1  == newRow) {
                //move right or left
                if(_Column - 1 == newColumn) {return true;}
                else if(_Column + 1 == newColumn) {return true;}
            }
        }
        return false;
    }



    // the enemy pawn attacks from the top of the board to the  bottom
    bool EnemyPawnAttack(int newColumn, int newRow) {
        if(newColumn >= 0 && newColumn < boardData.gridSize && newRow >= 0 && newRow < boardData.gridSize) {
            //normal movements
            if(_Row - 1  == newRow) {
                //move right or left
                if(_Column - 1 == newColumn) {return true;}
                else if(_Column + 1 == newColumn) {return true;}
            }
        }
        return false;
    }



    bool KingUnderAttack(int newColumn, int newRow) {
        // if the move is valid, check if king under attack
        if(checkValidMove(newColumn, newRow)) {

            // convert board and pieces to boardPosition data
            ChessBoardPosition boardPosition = new ChessBoardPosition();
            boardPosition.copyChessBoardBehavoir(boardData);
            ChessBoardPositionPieceData[] pieces = boardData.getPiecesOfColor(_Color);

            //get regerence to moved piece and set in board position
            ChessBoardPositionPieceData movedPiece = null;;
            for(int i = 0; i < pieces.Length; i++) {
                if(pieces[i].Column == _Column && pieces[i].Row == Row) {movedPiece = pieces[i];}
            }
            boardPosition.setLastPieceMoved(movedPiece);
            //Debug.Log("moved piece: " + movedPiece.Type + " " + movedPiece.Color);
            
            //move piece to newPosition
            movedPiece.movePiece(newColumn, newRow);

            //get reference to king in new board
            ChessBoardPositionPieceData king = null;
            for(int i = 0; i < pieces.Length; i++) {
                if(pieces[i].Type == PieceType.King) {king = pieces[i];}
            }


            // check if king under attack
            for(int i = 0; i < boardData.gridSize; i++) {
                for(int j = 0; j < boardData.gridSize; j++) {
                    if(boardData.pieceAtpos(i, j)) {
                        ChessBoardPositionPieceData enemyPiece = boardData.getPieceAtPos(i,j);
                        if(enemyPiece.Color != _Color) {
                            if(enemyPiece.checkValidAttack(king.Column, king.Row)) {return false;}
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}