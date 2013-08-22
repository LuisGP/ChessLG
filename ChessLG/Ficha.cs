using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Linq;
using System.Text;

namespace ChessLG
{
    public enum ValorFicha
    {
        PEON = 10,
        CABALLO = 30,
        ALFIL = 35,
        TORRE = 55,
        REINA = 100,
        REY = 999999
    }

    public class NotAlg
    {
        public const string Peon = "";
        public const string Torre = "T";
        public const string Caballo = "C";
        public const string Alfil = "A";
        public const string Reina = "D";
        public const string Rey = "R";
        public const string CAPTURA = "x";
        public const string JAQUE = "+";
        public const string MATE = "++";
        public const string ENROQUEC = "O-O";
        public const string ENROQUEL = "O-O-O";
        public const string TABLAS = "=";
        static public string[] columna = { "a", "b", "c", "d", "e", "f", "g", "h" };
        static public string[] fila = { "1", "2", "3", "4", "5", "6", "7", "8" };
    }

    public abstract class Ficha
    {
        public const bool BLANCA = true;
        public const bool NEGRA = false;

        public Texture2D textura;
        public ArrayList celdasAmenazadas;
        public ArrayList movimientosPermitidos;
        public ValorFicha valor;
        public Casilla miCasilla;
        public bool capturada;
        public bool color;
        public bool movida = false;
        public Casilla enroque = null;
        public string notacion;

        public override string ToString()
        {
            return notacion;
        }

        public Ficha(bool color, Casilla casilla)
        {
            celdasAmenazadas = new ArrayList();
            movimientosPermitidos = new ArrayList();
            capturada = false;
            this.miCasilla = casilla;
            this.color = color;
        }

        public void Draw(SpriteBatch sBatch, Vector2 pos, Color color)
        {
            sBatch.Draw(textura, pos, color);
        }

        public abstract void actualizarMovimientos(Casilla casilla);

        public abstract void actualizarAmenazas(Casilla casilla);

    }

    public class Peon : Ficha
    {
        public Casilla alPaso = null;
        public Casilla posNormal = null;
        public int mov = -1;

        public Peon(bool color, Casilla casilla)
            : base(color, casilla)
        {
            valor = ValorFicha.PEON;
            notacion = NotAlg.Peon;
            Casilla vecina, vecina2;

            if (color)
            {
                textura = Texturas.BLANCO_PEON;
                vecina = miCasilla.tablero.tablero[miCasilla.posX - 1][miCasilla.posY];
                vecina2 = miCasilla.tablero.tablero[miCasilla.posX - 2][miCasilla.posY];
            }
            else
            {
                textura = Texturas.NEGRO_PEON;
                vecina = miCasilla.tablero.tablero[miCasilla.posX + 1][miCasilla.posY];
                vecina2 = miCasilla.tablero.tablero[miCasilla.posX + 2][miCasilla.posY];
            }

            celdasAmenazadas.Add(vecina.izquierda);
            celdasAmenazadas.Add(vecina.derecha);
            movimientosPermitidos.Add(vecina);
            movimientosPermitidos.Add(vecina2);
            alPaso = vecina2;
            posNormal = vecina;
        }

        public override void actualizarMovimientos(Casilla casilla)
        {
            if(mov == -1 && casilla == alPaso)
                mov = miCasilla.tablero.nTurno;

            miCasilla = casilla;

            Casilla vecina = null;
            int x, y;

            movimientosPermitidos.Clear();

            // Al siguiente, me borro si no me comen
            if (mov != -1 && alPaso != null
                && miCasilla.tablero.nTurno - mov > 0
                && posNormal != null && !this.capturada)
            {
                posNormal.ficha = null;
                posNormal.alPaso = false;
                alPaso = null;
                posNormal = null;
            }

            x = miCasilla.posX;
            y = miCasilla.posY;

            if (color)
                x--;
            else
                x++;

            if (x >= 0 && x < 8)
                vecina = miCasilla.tablero.tablero[x][y];
            else
                return;

            // Movmiento normal
            if (vecina != null && vecina.ficha == null)
            {
                movimientosPermitidos.Add(vecina);
                if (!movida)
                    posNormal = vecina;
            }

            // ¿Puede comer?
            if (y > 0)
            {
                vecina = miCasilla.tablero.tablero[x][y - 1];
                if (vecina.ficha != null && vecina.ficha.color != color)
                {
                    movimientosPermitidos.Add(vecina);
                }
            }

            if (y < 7)
            {
                vecina = miCasilla.tablero.tablero[x][y + 1];
                if (vecina.ficha != null && vecina.ficha.color != color)
                {
                    movimientosPermitidos.Add(vecina);
                }
            }

            // Salida doble
            if (!movida)
            {
                if (color)
                    x--;
                else
                    x++;

                if (x >= 0 && x < 8)
                    vecina = miCasilla.tablero.tablero[x][y];

                if (vecina != null && vecina.ficha == null)
                {
                    movimientosPermitidos.Add(vecina);
                }
            }
        }

        public override void actualizarAmenazas(Casilla casilla)
        {
            miCasilla = casilla;

            Casilla vecina = null;
            int x, y;

            celdasAmenazadas.Clear();

            x = miCasilla.posX;
            y = miCasilla.posY;

            if (color)
            {
                x--;
                if (x < 0)
                    return;
            }
            else
            {
                x++;
                if (x > 7)
                    return;
            }

            if (y > 0)
            {
                vecina = miCasilla.tablero.tablero[x][y - 1];
                celdasAmenazadas.Add(vecina);
            }

            if (y < 7)
            {
                vecina = miCasilla.tablero.tablero[x][y + 1];
                celdasAmenazadas.Add(vecina);
            }
        }
    }

    public class Caballo : Ficha
    {
        public Caballo(bool color, Casilla casilla)
            : base(color, casilla)
        {
            valor = ValorFicha.CABALLO;
            notacion = NotAlg.Caballo;

            if (color)
            {
                textura = Texturas.BLANCO_CABALLO;
            }
            else
            {
                textura = Texturas.NEGRO_CABALLO;
            }

            actualizarMovimientos(miCasilla);
        }

        public override void actualizarMovimientos(Casilla casilla)
        {
            miCasilla = casilla;

            movimientosPermitidos.Clear();

            Casilla vecina;

            for (int i = 0; i < miCasilla.caballo.Length; i++)
            {
                vecina = miCasilla.caballo[i];
                if (vecina != null)
                {
                    if (vecina.ficha == null || vecina.ficha.color != color)
                    {
                        movimientosPermitidos.Add(vecina);
                    }
                }
            }
        }

        public override void actualizarAmenazas(Casilla casilla)
        {
            miCasilla = casilla;

            celdasAmenazadas.Clear();

            Casilla vecina;

            for (int i = 0; i < miCasilla.caballo.Length; i++)
            {
                vecina = miCasilla.caballo[i];
                if (vecina != null)
                {
                    celdasAmenazadas.Add(vecina);
                }
            }
        }
    }

    public class Alfil : Ficha
    {
        public Alfil(bool color, Casilla casilla)
            : base(color, casilla)
        {
            valor = ValorFicha.ALFIL;
            notacion = NotAlg.Alfil;

            if (color)
            {
                textura = Texturas.BLANCO_ALFIL;
            }
            else
            {
                textura = Texturas.NEGRO_ALFIL;
            }
        }

        public override void actualizarMovimientos(Casilla casilla)
        {
            miCasilla = casilla;
            movimientosPermitidos.Clear();
            Alfil.añadirMovyAme(miCasilla, movimientosPermitidos, true);
        }

        public override void actualizarAmenazas(Casilla casilla)
        {
            miCasilla = casilla;
            celdasAmenazadas.Clear();
            Alfil.añadirMovyAme(miCasilla, celdasAmenazadas, false);
        }

        static public void añadirMovyAme(Casilla casilla, ArrayList al, bool mov)
        {
            Casilla vecina;

            // Arriba Derecha
            int j = casilla.posY + 1;
            int i;
            for (i = casilla.posX + 1; i < 8 && j < 8; i++)
            {
                vecina = casilla.tablero.tablero[i][j];

                if(Alfil.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;

                j++;
            }

            // Arriba Izquierda
            j = casilla.posY - 1;
            for (i = casilla.posX + 1; i < 8 && j >= 0; i++)
            {
                vecina = casilla.tablero.tablero[i][j];

                if (Alfil.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;

                j--;
            }

            // Abajo Izquierda
            j = casilla.posY - 1;
            for (i = casilla.posX - 1; i >= 0 && j >= 0; i--)
            {
                vecina = casilla.tablero.tablero[i][j];

                if (Alfil.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;

                j--;
            }

            // Abajo Derecha
            j = casilla.posY + 1;
            for (i = casilla.posX - 1; i >= 0 && j < 8; i--)
            {
                vecina = casilla.tablero.tablero[i][j];

                if (Alfil.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;

                j++;
            }
        }

        // Devuelve cuando el bucle debe parar
        static private bool comprobarMovAme(Casilla vecina, bool color, ArrayList al, bool mov)
        {
            if (vecina.ficha == null) // Puede mover y amenaza
            {
                al.Add(vecina);
                return false;
            }
            else
            {
                if (vecina.ficha.color != color) // Puede mover y amenaza
                {
                    al.Add(vecina);
                }
                else
                {
                    if (!mov) // Solo amenaza pero no puede mover
                    {
                        al.Add(vecina);
                    }
                }

                return true;
            }
        }
    }

    public class Torre : Ficha
    {
        public Torre(bool color, Casilla casilla)
            : base(color, casilla)
        {
            valor = ValorFicha.TORRE;
            notacion = NotAlg.Torre;

            if (color)
            {
                textura = Texturas.BLANCO_TORRE;
            }
            else
            {
                textura = Texturas.NEGRO_TORRE;
            }
        }

        static public void añadirMovyAme(Casilla casilla, ArrayList al, bool mov)
        {
            // Linea horizontal
            Casilla vecina;

            // Linea horizontal >
            for (int i = casilla.posX + 1; i < 8; i++)
            {
                vecina = casilla.tablero.tablero[i][casilla.posY];
                if(Torre.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;
            }

            // Linea horizontal <
            for (int i = casilla.posX - 1; i >= 0; i--)
            {
                vecina = casilla.tablero.tablero[i][casilla.posY];
                if (Torre.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;
            }

            // Linea vertical >
            for (int i = casilla.posY + 1; i < 8; i++)
            {
                vecina = casilla.tablero.tablero[casilla.posX][i];
                if (Torre.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;
            }

            // Linea vertical <
            for (int i = casilla.posY - 1; i >= 0; i--)
            {
                vecina = casilla.tablero.tablero[casilla.posX][i];
                if (Torre.comprobarMovAme(vecina, casilla.ficha.color, al, mov))
                    break;
            }
        }

        // Devuelve cuando el bucle debe parar
        static private bool comprobarMovAme(Casilla vecina, bool color, ArrayList al, bool mov)
        {            
            if (vecina.ficha == null) // Puede mover y amenaza
            {
                al.Add(vecina);
                return false;
            }
            else
            {
                if (vecina.ficha.color != color) // Puede mover y amenaza
                {
                    al.Add(vecina);
                }
                else
                {
                    if (!mov) // Solo amenaza pero no puede mover
                    {
                        al.Add(vecina);
                    }
                }

                return true;
            }
        }

        public override void actualizarMovimientos(Casilla casilla)
        {
            miCasilla = casilla;
            movimientosPermitidos.Clear();
            Torre.añadirMovyAme(miCasilla, movimientosPermitidos, true);
        }

        public override void actualizarAmenazas(Casilla casilla)
        {
            miCasilla = casilla;
            celdasAmenazadas.Clear();
            Torre.añadirMovyAme(miCasilla, celdasAmenazadas, false);
        }
    }

    public class Reina : Ficha
    {
        public Reina(bool color, Casilla casilla)
            : base(color, casilla)
        {
            valor = ValorFicha.REINA;
            notacion = NotAlg.Reina;

            if (color)
            {
                textura = Texturas.BLANCO_REINA;
            }
            else
            {
                textura = Texturas.NEGRO_REINA;
            }
        }

        public override void actualizarMovimientos(Casilla casilla)
        {
            miCasilla = casilla;
            movimientosPermitidos.Clear();
            Torre.añadirMovyAme(miCasilla, movimientosPermitidos, true);
            Alfil.añadirMovyAme(miCasilla, movimientosPermitidos, true);
        }

        public override void actualizarAmenazas(Casilla casilla)
        {
            miCasilla = casilla;
            celdasAmenazadas.Clear();
            Torre.añadirMovyAme(miCasilla, celdasAmenazadas, false);
            Alfil.añadirMovyAme(miCasilla, celdasAmenazadas, false);
        }
    }

    public class Rey : Ficha
    {
        public Torre torreC;
        public Torre torreL;

        public Rey(bool color, Casilla casilla)
            : base(color, casilla)
        {
            valor = ValorFicha.REY;
            notacion = NotAlg.Rey;

            if (color)
            {
                textura = Texturas.BLANCO_REY;
            }
            else
            {
                textura = Texturas.NEGRO_REY;
            }

            // Deben haberse posiionado previamente!!
            torreC = (Torre)casilla.tablero.tablero[casilla.posX][7].ficha;
            torreL = (Torre)casilla.tablero.tablero[casilla.posX][0].ficha;
        }

        public override void actualizarMovimientos(Casilla casilla)
        {
            miCasilla = casilla;

            ArrayList listaEnemigos;

            if (color == BLANCA)
                listaEnemigos = miCasilla.tablero.fichasNegras;
            else
                listaEnemigos = miCasilla.tablero.fichasBlancas;

            movimientosPermitidos.Clear();

            Casilla vecina;
            bool amenazada;

            for (int i = 0; i < miCasilla.rey.Length; i++)
            {
                vecina = miCasilla.rey[i];
                if (vecina != null)
                {
                    if (vecina.ficha == null || vecina.ficha.color != color)
                    {
                        amenazada = false;

                        for (int j = 0; j < listaEnemigos.Count; j++)
                        {
                            if (((Ficha)listaEnemigos[j]).celdasAmenazadas.Contains(vecina))
                            {
                                amenazada = true;
                                break;
                            }
                        }

                        if (!amenazada)
                        {
                            movimientosPermitidos.Add(vecina);
                        }
                    }
                }
            }

            // Enroque?
            enroque = null;
            torreC.enroque = null;
            torreL.enroque = null;
            Casilla vecina2, vecina3;
            if (movida == false)
            {
                if (torreC.movida == false)
                {
                    vecina = miCasilla.derecha;
                    vecina2 = vecina.derecha;
                    if (vecina.ficha == null && !miCasilla.tablero.jaqueCasilla(vecina, !color)
                        && vecina2.ficha == null && !miCasilla.tablero.jaqueCasilla(vecina2, !color)
                        && !miCasilla.tablero.esJaque(color, false))
                    {
                        movimientosPermitidos.Add(vecina2);
                        enroque = vecina2;
                        torreC.enroque = vecina;
                    }
                }

                if (torreL.movida == false)
                {
                    vecina = miCasilla.izquierda;
                    vecina2 = vecina.izquierda;
                    vecina3 = vecina2.izquierda;
                    if (vecina.ficha == null && !miCasilla.tablero.jaqueCasilla(vecina, !color)
                        && vecina2.ficha == null && !miCasilla.tablero.jaqueCasilla(vecina2, !color)
                        && vecina3.ficha == null && !miCasilla.tablero.jaqueCasilla(vecina3, !color)
                        && !miCasilla.tablero.esJaque(color, false))
                    {
                        movimientosPermitidos.Add(vecina2);
                        enroque = vecina2;
                        torreL.enroque = vecina;
                    }
                }
            }
        }

        public override void actualizarAmenazas(Casilla casilla)
        {
            miCasilla = casilla;

            celdasAmenazadas.Clear();

            Casilla vecina;

            for (int i = 0; i < miCasilla.rey.Length; i++)
            {
                vecina = miCasilla.rey[i];
                if (vecina != null)
                {
                    celdasAmenazadas.Add(vecina);
                }
            }
        }
    }
}
