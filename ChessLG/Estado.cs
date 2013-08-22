using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLG
{
    public class Estado
    {
        public const int LADO = 8;
        public const int Vacio = 0;
        public const int PeonBlanco = 1;
        public const int TorreBlanca = 2;
        public const int CaballoBlanco = 3;
        public const int AlfilBlanco = 4;
        public const int ReinaBlanca = 5;
        public const int ReyBlanco = 6;
        public const int PeonNegro = 7;
        public const int TorreNegra = 8;
        public const int CaballoNegro = 9;
        public const int AlfilNegro = 10;
        public const int ReinaNegra = 11;
        public const int ReyNegro = 12;

        public int[][] tablero;

        // Otros datos de la partida (info Extendida)
        public bool turno;
        public int nTurno;
        public bool[][] alPaso; // Casilla
        public bool[][] movida; // Ficha
        public int[][] mov; // Peon - alPaso

        public int[] torreCB; // Coords X e Y
        public int[] torreLB;
        public int[] torreCN;
        public int[] torreLN;

        public Estado()
        {
            tablero = new int[LADO][];
            alPaso = new bool[LADO][];
            movida = new bool[LADO][];
            mov = new int[LADO][];

            turno = Ficha.BLANCA;
            nTurno = 0;

            torreCB = new int[2];
            torreLB = new int[2];
            torreCN = new int[2];
            torreLN = new int[2];

            for (int i = 0; i < LADO; i++)
            {
                tablero[i] = new int[LADO];
                alPaso[i] = new bool[LADO];
                movida[i] = new bool[LADO];
                mov[i] = new int[LADO];

                for (int j = 0; j < LADO; j++)
                {
                    tablero[i][j] = Estado.Vacio;
                    alPaso[i][j] = false;
                    movida[i][j] = false;
                    mov[i][j] = -1;
                }
            }
        }

        public Estado(Tablero t) : this()
        {
            cargarEstado(t);
        }

        public void cargarEstado(Tablero t){
            int pieza, x, y;

            turno = t.turno;
            nTurno = t.nTurno;

            foreach (Ficha f in t.fichasBlancas)
            {
                x = f.miCasilla.posX;
                y = f.miCasilla.posY;

                switch (f.valor)
                {
                    case ValorFicha.PEON:
                        pieza = Estado.PeonBlanco;
                        mov[x][y] = ((Peon)f).mov;
                        alPaso[x][y] = ((Peon)f).alPaso != null;
                        break;
                    case ValorFicha.CABALLO:
                        pieza = Estado.CaballoBlanco;
                        break;
                    case ValorFicha.ALFIL:
                        pieza = Estado.AlfilBlanco;
                        break;
                    case ValorFicha.TORRE:
                        pieza = Estado.TorreBlanca;
                        break;
                    case ValorFicha.REINA:
                        pieza = Estado.ReinaBlanca;
                        break;
                    case ValorFicha.REY:
                        pieza = Estado.ReyBlanco;
                        torreCB[0] = t.reyBlanco.torreC.miCasilla.posX;
                        torreCB[1] = t.reyBlanco.torreC.miCasilla.posY;
                        torreLB[0] = t.reyBlanco.torreL.miCasilla.posX;
                        torreLB[1] = t.reyBlanco.torreL.miCasilla.posY;
                        break;
                    default:
                        pieza = Estado.Vacio;
                        break;
                }

                movida[x][y] = f.movida;
                tablero[x][y] = pieza;
            }

            foreach (Ficha f in t.fichasNegras)
            {
                x = f.miCasilla.posX;
                y = f.miCasilla.posY;

                switch (f.valor)
                {
                    case ValorFicha.PEON:
                        pieza = Estado.PeonNegro;
                        mov[x][y] = ((Peon)f).mov;
                        alPaso[x][y] = ((Peon)f).alPaso != null;
                        break;
                    case ValorFicha.CABALLO:
                        pieza = Estado.CaballoNegro;
                        break;
                    case ValorFicha.ALFIL:
                        pieza = Estado.AlfilNegro;
                        break;
                    case ValorFicha.TORRE:
                        pieza = Estado.TorreNegra;
                        break;
                    case ValorFicha.REINA:
                        pieza = Estado.ReinaNegra;
                        break;
                    case ValorFicha.REY:
                        pieza = Estado.ReyNegro;
                        torreCN[0] = t.reyNegro.torreC.miCasilla.posX;
                        torreCN[1] = t.reyNegro.torreC.miCasilla.posY;
                        torreLN[0] = t.reyNegro.torreL.miCasilla.posX;
                        torreLN[1] = t.reyNegro.torreL.miCasilla.posY;
                        break;
                    default:
                        pieza = Estado.Vacio;
                        break;
                }

                movida[x][y] = f.movida;
                tablero[x][y] = pieza;
            }
        }
    }
}
