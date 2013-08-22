using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Collections;

namespace ChessLG
{

    public class Casilla
    {
        public const int LADO = 128;

        public const bool BLANCO = true;
        public const bool NEGRO = false;

        public string notacion;
        public Tablero tablero;
        public Texture2D textura;
        public Casilla arriba = null;
        public Casilla abajo = null;
        public Casilla izquierda = null;
        public Casilla derecha = null;
        public Casilla caballo1 = null;
        public Casilla caballo2 = null;
        public Casilla caballo4 = null;
        public Casilla caballo5 = null;
        public Casilla caballo7 = null;
        public Casilla caballo8 = null;
        public Casilla caballo10 = null;
        public Casilla caballo11 = null;
        public Casilla arribaDerecha = null;
        public Casilla abajoDerecha = null;
        public Casilla abajoIzquierda = null;
        public Casilla arribaIzquierda = null;

        public Casilla[] caballo = new Casilla[8];
        public Casilla[] rey = new Casilla[8];

        public bool selected = false;

        public Vector2 pos;
        public int posX;
        public int posY;

        public Ficha ficha;
        public bool alPaso = false;

        public override string ToString()
        {
            return notacion;
        }

        public Casilla(bool color, Tablero tablero, int i, int j)
        {
            if (color)
                textura = Texturas.CASILLA_BLANCA;
            else
                textura = Texturas.CASILLA_NEGRA;

            this.tablero = tablero;

            pos = new Vector2(j * LADO, i * LADO);
            posX = i;
            posY = j;
            notacion = NotAlg.columna[j] + NotAlg.fila[i];
        }

        public void calcularVecinos(){
            int i = posX;
            int j = posY;

            // Calculemos sus vecinos - Por claridad, no agrupamos
            if (i > 0)
                arriba = tablero.tablero[i - 1][j];
            if (i < 7)
                abajo = tablero.tablero[i + 1][j];
            if (j > 0)
                izquierda = tablero.tablero[i][j - 1];
            if (j < 7)
                derecha = tablero.tablero[i][j + 1];
            if (i > 0 && j > 0)
                arribaIzquierda = tablero.tablero[i - 1][j - 1];
            if (i > 0 && j < 7)
                arribaDerecha = tablero.tablero[i - 1][j + 1];
            if (i < 7 && j > 0)
                abajoIzquierda = tablero.tablero[i + 1][j - 1];
            if (i < 7 && j < 7)
                abajoDerecha = tablero.tablero[i + 1][j + 1];
            // Caballos
            if (i > 1 && j < 7)
                caballo1 = tablero.tablero[i - 2][j + 1];
            if (i > 0 && j < 6)
                caballo2 = tablero.tablero[i - 1][j + 2];
            if (i < 7 && j < 6)
                caballo4 = tablero.tablero[i + 1][j + 2];
            if (i < 6 && j < 7)
                caballo5 = tablero.tablero[i + 2][j + 1];
            if (i < 6 && j > 0)
                caballo7 = tablero.tablero[i + 2][j - 1];
            if (i < 7 && j > 1)
                caballo8 = tablero.tablero[i + 1][j - 2];
            if (i > 0 && j > 1)
                caballo10 = tablero.tablero[i - 1][j - 2];
            if (i > 1 && j > 0)
                caballo11 = tablero.tablero[i - 2][j - 1];

            caballo[0] = caballo1;
            caballo[1] = caballo2;
            caballo[2] = caballo4;
            caballo[3] = caballo5;
            caballo[4] = caballo7;
            caballo[5] = caballo8;
            caballo[6] = caballo10;
            caballo[7] = caballo11;

            rey[0] = arriba;
            rey[1] = abajo;
            rey[2] = derecha;
            rey[3] = izquierda;
            rey[4] = arribaIzquierda;
            rey[5] = arribaDerecha;
            rey[6] = abajoDerecha;
            rey[7] = abajoIzquierda;

        }

        public void Draw(SpriteBatch sBatch)
        {
            Color color = Color.White;

            if (selected)
                color = Color.Red;

            pintarAmenazas(sBatch);

            //sBatch.Draw(textura, pos, Color.White);

            if(ficha != null && !alPaso)
                ficha.Draw(sBatch, pos, color);
        }

        // De prueba
        public void pintarAmenazas(SpriteBatch sBatch)
        {
            ArrayList enemigas;
            bool ame;

            if (tablero.turno == Ficha.BLANCA)
                enemigas = tablero.fichasNegras;
            else
                enemigas = tablero.fichasBlancas;

            ame = false;
            for (int i = 0; i < enemigas.Count; i++)
            {
                if (((Ficha)enemigas[i]).celdasAmenazadas.Contains(this))
                {
                    ame = true;
                    break;
                }
            }

            if(textura == Texturas.CASILLA_BLANCA)
                if(ame)
                    sBatch.Draw(textura, pos, Color.Yellow);
                else
                    sBatch.Draw(textura, pos, Color.White);
            else
                if (ame)
                    sBatch.Draw(Texturas.CASILLA_BLANCA, pos, Color.YellowGreen);
                else
                    sBatch.Draw(textura, pos, Color.White);

        }
    }
}
