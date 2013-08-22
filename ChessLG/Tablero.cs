using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace ChessLG
{
    public class Tablero
    {
        public Casilla[][] tablero;
        public bool turno = Ficha.BLANCA;
        public int nTurno = 0;

        public ArrayList fichasBlancas = new ArrayList();
        public ArrayList fichasNegras = new ArrayList();
        public Rey reyBlanco;
        public Rey reyNegro;

        private Casilla seleccionada;

        public ArrayList historial = new ArrayList();
        public string movimiento = "";

        public Casilla selected 
        { 
            get
            {
                return seleccionada;
            } 
            set
            {
                // Turnos
                if (value != null && value.ficha != null && value.ficha.color != turno)
                {
                    if((seleccionada == null || seleccionada.ficha == null)
                        || (seleccionada.ficha != null
                        && !seleccionada.ficha.movimientosPermitidos.Contains(value)))
                            return;
                }

                if(seleccionada != null)
                    seleccionada.selected = false;

                seleccionada = value;

                if(value != null && seleccionada != null)
                    seleccionada.selected = true;

            } 
        }

        public Ficha click(double x, double y)
        {
            int i = (int)(y / Casilla.LADO);
            int j = (int)(x / Casilla.LADO);

            if (i >= 0 && i < 8 && j >= 0 && j < 8)
            {
                if (selected == tablero[i][j])
                {
                    return selected.ficha;
                }

                Ficha previa = null;
                Casilla origen = null;

                if (selected != null)
                {
                    previa = selected.ficha;
                    origen = selected;
                }

                selected = tablero[i][j];

                if (selected == null)
                    return null;

                if(moverFicha(previa, origen, selected))
                {
                    // Proximo turno
                    turno = !turno;
                    nTurno++;
                    selected = null;
                    return null;
                }

                if(selected != null)
                    return selected.ficha;
            }

            return null;
        }

        private string distinguirFicha(Ficha previa, Casilla destino)
        {
            ArrayList j;

            if (previa.color == Ficha.BLANCA)
                j = fichasBlancas;
            else
                j = fichasNegras;

            foreach (Ficha f in j)
            {
                if (f != previa && f.valor == previa.valor)
                {
                    if (f.movimientosPermitidos.Contains(destino))
                    {
                        if (f.miCasilla.posX != previa.miCasilla.posX)
                        {
                            return NotAlg.fila[previa.miCasilla.posX];
                        }
                        else
                        {
                            return NotAlg.columna[previa.miCasilla.posY];
                        }
                    }
                }
            }

            return "";
        }

        public bool moverFicha(Ficha previa, Casilla origen, Casilla destino)
        {
            bool enroque = false;

            if (turno == Ficha.BLANCA)
                movimiento = ((nTurno / 2) + 1).ToString() + ". ";
            else
                movimiento += " ";

            if (previa != null)
            {
                if (previa.movimientosPermitidos.Contains(destino))
                {
                    Ficha comida = null;
                    ArrayList jugador = null;
                    bool mov = previa.movida;
                    bool paso = destino.alPaso;

                    movimiento += previa.ToString();

                    // Pueden ir alli varias piezas mias???
                    movimiento += distinguirFicha(previa, destino);

                    if (destino.ficha != null)
                    {
                        destino.ficha.capturada = true;
                        comida = destino.ficha;
                        if (fichasBlancas.Contains(destino.ficha))
                        {
                            fichasBlancas.Remove(destino.ficha);
                            jugador = fichasBlancas;
                            movimiento += "x";
                        }
                        else if (fichasNegras.Contains(destino.ficha))
                        {
                            fichasNegras.Remove(destino.ficha);
                            jugador = fichasNegras;
                            movimiento += "x";
                        }

                        // ¿Me comen al paso? Solo peones...
                        try
                        {
                            Peon peon = (Peon)previa;

                            if (destino.ficha.miCasilla != destino)
                            {
                                destino.ficha.miCasilla.ficha = null;
                                destino.alPaso = false;
                            }
                        }
                        catch (Exception)
                        {
                            destino.alPaso = false;
                        }

                        // Me comen habiendo dado doble salto?
                        try
                        {
                            Peon peon = (Peon)destino.ficha;

                            if (peon.alPaso == destino)
                            {
                                peon.posNormal.alPaso = false;
                                peon.posNormal.ficha = null;
                            }
                        }
                        catch (Exception)
                        {
                        }

                    }

                    movimiento += destino.ToString();

                    destino.ficha = previa;
                    origen.ficha = null;

                    //selected.ficha.actualizarMovimientos(selected);
                    previa.movida = true;
                   
                    // Fue un enroque?
                    Torre torre = null;
                    Casilla casT = null;
                    if (previa.enroque == destino)
                    {
                        try
                        {
                            // Si no es Rey, no fue enroque y saltara la excepcion :)
                            Rey r = (Rey)previa;

                            if (destino.derecha == r.torreC.miCasilla)
                            {
                                casT = r.torreC.miCasilla;
                                moverFicha(r.torreC, r.torreC.miCasilla, r.torreC.enroque);
                                torre = r.torreC;
                                movimiento = ((nTurno / 2) + 1).ToString() + ". " + NotAlg.ENROQUEC;
                            }
                            else
                            {
                                casT = r.torreL.miCasilla;
                                moverFicha(r.torreL, r.torreL.miCasilla, r.torreL.enroque);
                                torre = r.torreL;
                                movimiento = ((nTurno / 2) + 1).ToString() + ". " + NotAlg.ENROQUEL;
                            }

                            enroque = true;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    // Fue salto doble de peon?
                    try
                    {
                        Peon peon = (Peon)previa;

                        if (peon.alPaso == destino)
                        {
                            peon.posNormal.ficha = peon;
                            peon.posNormal.alPaso = true;
                        }
                        else
                        {
                            if (peon.posNormal != null)
                            {
                                peon.posNormal.alPaso = false;
                            }
                            peon.alPaso = null;
                            peon.posNormal = null;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    actualizarMovimientos();

                    // Evitar autojaque
                    if (esJaque(turno, false))
                    {
                        // Por si acaso
                        destino.ficha = null;
                        destino.alPaso = paso;

                        // Deshacemos y nos sigue tocando
                        if (comida != null && jugador != null)
                        {
                            comida.capturada = false;
                            jugador.Add(comida);
                            destino.ficha = comida;
                        }

                        origen.ficha = previa;
                        previa.movida = mov;

                        // Volver a poner variables de "al paso"
                        try
                        {
                            Peon peon = (Peon)previa;
                            if (peon.alPaso == destino)
                            {
                                peon.posNormal.ficha = null;
                                peon.posNormal.alPaso = false;
                                peon.mov = -1;
                            }
                        }
                        catch (Exception)
                        {
                        }

                        // Si fue enroque, deshacer tambien a la torre
                        // Aunque nunca deberia darse, pues se comprueba
                        // antes que el "camino" no estuviese amenazado
                        if (enroque && casT != null && torre != null)
                        {
                            torre.miCasilla = null;
                            casT.ficha = torre;
                            torre.miCasilla = casT;
                        }

                        actualizarMovimientos();

                        // Si movemos, deseleccionamos
                        destino.selected = false;
                        return false;
                    }

                    // Finalmente, promocionamos un peon?
                    try
                    {
                        Peon peon = (Peon)previa;
                        if ((peon.color && peon.miCasilla.posX == 0)
                            || (!peon.color && peon.miCasilla.posX == 7))
                        {
                            Promocion prom = new Promocion(peon);
                            prom.ShowDialog();

                            if (peon.color)
                            {
                                fichasBlancas.Remove(peon);
                                fichasBlancas.Add(prom.seleccionada);
                            }
                            else
                            {
                                fichasNegras.Remove(peon);
                                fichasNegras.Add(prom.seleccionada);
                            }

                            movimiento += prom.seleccionada.ToString();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    // Si movemos, deseleccionamos
                    destino.selected = false;

                    return true;
                }
                return false;
            }
            return false;
        }

        public Tablero()
        {
            // Creamos el tablero
            tablero = new Casilla[8][];

            for (int i = 0; i < tablero.Length; i++)
            {
                tablero[i] = new Casilla[8];

                for (int j = 0; j < tablero[i].Length; j++)
                {
                    bool color;

                    color = (i + j) % 2 == 0;

                    tablero[i][j] = new Casilla(color, this, i, j);
                }
            }

            // Inicializamos vecinos
            for (int i = 0; i < tablero.Length; i++)
            {
                for (int j = 0; j < tablero[i].Length; j++)
                {
                    tablero[i][j].calcularVecinos();
                }
            }

            // Situamos las fichas
            // Peones
            for (int i = 0; i < tablero[6].Length; i++)
            {
                tablero[6][i].ficha = new Peon(Ficha.BLANCA, tablero[6][i]);
                tablero[1][i].ficha = new Peon(Ficha.NEGRA, tablero[1][i]);
            }
            // Torres
            tablero[0][0].ficha = new Torre(Ficha.NEGRA, tablero[0][0]);
            tablero[0][7].ficha = new Torre(Ficha.NEGRA, tablero[0][7]);
            tablero[7][0].ficha = new Torre(Ficha.BLANCA, tablero[7][0]);
            tablero[7][7].ficha = new Torre(Ficha.BLANCA, tablero[7][7]);
            // Caballos
            tablero[0][1].ficha = new Caballo(Ficha.NEGRA, tablero[0][1]);
            tablero[0][6].ficha = new Caballo(Ficha.NEGRA, tablero[0][6]);
            tablero[7][1].ficha = new Caballo(Ficha.BLANCA, tablero[7][1]);
            tablero[7][6].ficha = new Caballo(Ficha.BLANCA, tablero[7][6]);
            // Alfiles
            tablero[0][2].ficha = new Alfil(Ficha.NEGRA, tablero[0][2]);
            tablero[0][5].ficha = new Alfil(Ficha.NEGRA, tablero[0][5]);
            tablero[7][2].ficha = new Alfil(Ficha.BLANCA, tablero[7][2]);
            tablero[7][5].ficha = new Alfil(Ficha.BLANCA, tablero[7][5]);
            // Reinas
            tablero[0][3].ficha = new Reina(Ficha.NEGRA, tablero[0][3]);
            tablero[7][3].ficha = new Reina(Ficha.BLANCA, tablero[7][3]);
            // Reyes
            tablero[0][4].ficha = new Rey(Ficha.NEGRA, tablero[0][4]);
            tablero[7][4].ficha = new Rey(Ficha.BLANCA, tablero[7][4]);

            // Añadimos cada ficha a su jugador
            for (int i = 0; i < tablero[0].Length; i++)
            {
                fichasBlancas.Add(tablero[6][i].ficha);
                fichasBlancas.Add(tablero[7][i].ficha);
                fichasNegras.Add(tablero[1][i].ficha);
                fichasNegras.Add(tablero[0][i].ficha);
            }

            reyBlanco = (Rey)tablero[7][4].ficha;
            reyNegro = (Rey)tablero[0][4].ficha;

            historial.Add("");
        }

        // Determina si una casilla esta amenazada por el "color"
        public bool jaqueCasilla(Casilla c, bool color)
        {
            ArrayList jugador = new ArrayList();

            if (color)
            {
                jugador = fichasBlancas;
            }
            else
            {
                jugador = fichasNegras;
            }

            Ficha ficha;
            for (int i = 0; i < jugador.Count; i++)
            {
                ficha = (Ficha)jugador[i];

                if (ficha.celdasAmenazadas.Contains(c))
                    return true;
            }

            return false;
        }

        // Determina si el rey "color" está en jaque (mate)
        public bool esJaque(bool color, bool mate)
        {
            Rey rey;
            ArrayList jugadorRival;
            Tablero tablero = new Tablero();

            tablero.setEstado(this.getEstado());

            // Copia del jugador (se modifica en los calculos)
            ArrayList jugador = new ArrayList();

            if (color)
            {
                jugadorRival = tablero.fichasNegras;
                for (int i = 0; i < tablero.fichasBlancas.Count; i++)
                    jugador.Add(tablero.fichasBlancas[i]);
                rey = tablero.reyBlanco;
            }
            else
            {
                jugadorRival = tablero.fichasBlancas;
                for (int i = 0; i < tablero.fichasNegras.Count; i++)
                    jugador.Add(tablero.fichasNegras[i]);
                rey = tablero.reyNegro;
            }

            Ficha ficha;

            // Copia de los movimientos (se modifica en los calculos)
            ArrayList movimientos = new ArrayList();

            bool noJaque = false;
            for (int i = 0; i < jugadorRival.Count; i++)
            {
                ficha = (Ficha)jugadorRival[i];

                if (ficha.celdasAmenazadas.Contains(rey.miCasilla))
                {
                    if (!mate)
                        return true;

                    if (rey.movimientosPermitidos.Count == 0)
                    {
                        Casilla aux;
                        Ficha f, r;

                        for (int j = 0; j < jugador.Count; j++)
                        {
                            f = (Ficha)jugador[j];
                            aux = f.miCasilla;
                            movimientos.Clear();

                            for (int k = 0; k < f.movimientosPermitidos.Count; k++)
                            {
                                movimientos.Add(f.movimientosPermitidos[k]);
                            }

                            foreach (Casilla c in movimientos)
                            {
                                r = c.ficha;

                                if (tablero.moverFicha(f, aux, c))
                                {
                                    if (!tablero.esJaque(color, false))
                                        noJaque = true;
                                }

                                c.ficha = r;
                                if (r != null)
                                    r.capturada = false;
                                aux.ficha = f;
                                tablero.actualizarMovimientos();

                                if (noJaque)
                                    return false;
                            }
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        // Determina si un jugador está ahogado (tablas)
        public bool ahogado(bool color)
        {
            ArrayList jugador;
            Rey rey;
            Tablero tablero = new Tablero();

            tablero.setEstado(this.getEstado());

            if (color)
            {
                jugador = tablero.fichasBlancas;
                rey = reyBlanco;
            }
            else
            {
                jugador = tablero.fichasNegras;
                rey = reyNegro;
            }

            if (tablero.esJaque(color, true))
                return false;

            Ficha f;
            for (int i = 0; i < jugador.Count; i++)
            {
                f = (Ficha)jugador[i];
                if (f.movimientosPermitidos.Count > 0)
                    return false;
            }

            return true;
        }

        public void actualizarAmenazas()
        {
            Ficha f = null;

            for (int i = 0; i < tablero.Length; i++)
            {
                for (int j = 0; j < tablero[i].Length; j++)
                {
                    f = tablero[i][j].ficha;
                    if (f != null && !tablero[i][j].alPaso)
                    {
                        f.actualizarAmenazas(tablero[i][j]);
                    }
                }
            }
        }

        public void actualizarMovimientos()
        {
            Ficha f = null;

            // Asegurarse siempre actualizando amenazas
            actualizarAmenazas();

            for (int i = 0; i < tablero.Length; i++)
            {
                for (int j = 0; j < tablero[i].Length; j++)
                {
                    f = tablero[i][j].ficha;
                    if (f != null && !tablero[i][j].alPaso)
                    {
                        f.actualizarMovimientos(tablero[i][j]);
                    }
                }
            }
        }

        public void Draw(SpriteBatch sBatch)
        {
            for (int i = 0; i < tablero.Length; i++)
            {
                for (int j = 0; j < tablero[i].Length; j++)
                {
                    tablero[i][j].Draw(sBatch);
                }
            }

        }

        public Estado getEstado()
        {
            return new Estado(this);
        }

        public void setEstado(Estado estado)
        {
            try
            {
                turno = estado.turno;
                nTurno = estado.nTurno;

                fichasBlancas.Clear();
                fichasNegras.Clear();
                selected = null;
                seleccionada = null;

                for (int i = 0; i < Estado.LADO; i++)
                {
                    for (int j = 0; j < Estado.LADO; j++)
                    {
                        // Referente a la casilla
                        tablero[i][j].alPaso = estado.alPaso[i][j];

                        switch (estado.tablero[i][j])
                        {
                            case Estado.Vacio:
                                tablero[i][j].ficha = null;
                                break;

                            case Estado.PeonBlanco:
                                Peon peon = new Peon(Ficha.BLANCA, tablero[i][j]);
                                tablero[i][j].ficha = peon;
                                peon.mov = estado.mov[i][j];
                                if (tablero[i][j].abajo.alPaso)
                                {
                                    peon.alPaso = tablero[i][j];
                                    peon.posNormal = tablero[i][j].abajo;
                                    peon.posNormal.ficha = peon;
                                }
                                break;
                            case Estado.TorreBlanca:
                                Torre torre = new Torre(Ficha.BLANCA, tablero[i][j]);
                                tablero[i][j].ficha = torre;
                                break;
                            case Estado.CaballoBlanco:
                                Caballo caballo = new Caballo(Ficha.BLANCA, tablero[i][j]);
                                tablero[i][j].ficha = caballo;
                                break;
                            case Estado.AlfilBlanco:
                                Alfil alfil = new Alfil(Ficha.BLANCA, tablero[i][j]);
                                tablero[i][j].ficha = alfil;
                                break;
                            case Estado.ReinaBlanca:
                                Reina dama = new Reina(Ficha.BLANCA, tablero[i][j]);
                                tablero[i][j].ficha = dama;
                                break;
                            case Estado.ReyBlanco:
                                reyBlanco = new Rey(Ficha.BLANCA, tablero[i][j]);
                                tablero[i][j].ficha = reyBlanco;
                                break;

                            case Estado.PeonNegro:
                                Peon peon2 = new Peon(Ficha.NEGRA, tablero[i][j]);
                                tablero[i][j].ficha = peon2;
                                peon2.mov = estado.mov[i][j];
                                if (tablero[i][j].arriba.alPaso)
                                {
                                    peon2.alPaso = tablero[i][j];
                                    peon2.posNormal = tablero[i][j].arriba;
                                    peon2.posNormal.ficha = peon2;
                                }
                                break;
                            case Estado.TorreNegra:
                                Torre torre2 = new Torre(Ficha.NEGRA, tablero[i][j]);
                                tablero[i][j].ficha = torre2;
                                break;
                            case Estado.CaballoNegro:
                                Caballo caballo2 = new Caballo(Ficha.NEGRA, tablero[i][j]);
                                tablero[i][j].ficha = caballo2;
                                break;
                            case Estado.AlfilNegro:
                                Alfil alfil2 = new Alfil(Ficha.NEGRA, tablero[i][j]);
                                tablero[i][j].ficha = alfil2;
                                break;
                            case Estado.ReinaNegra:
                                Reina dama2 = new Reina(Ficha.NEGRA, tablero[i][j]);
                                tablero[i][j].ficha = dama2;
                                break;
                            case Estado.ReyNegro:
                                reyNegro = new Rey(Ficha.NEGRA, tablero[i][j]);
                                tablero[i][j].ficha = reyNegro;
                                break;

                            default:
                                break;
                        }

                        if (tablero[i][j].ficha != null)
                        {
                            tablero[i][j].ficha.movida = estado.movida[i][j];
                            if (tablero[i][j].ficha.color == Ficha.BLANCA)
                            {
                                fichasBlancas.Add(tablero[i][j].ficha);
                            }
                            else
                            {
                                fichasNegras.Add(tablero[i][j].ficha);
                            }
                        }
                    }
                } // Bucle externo (i)

                // Torres de los reyes
                reyBlanco.torreC = ((Torre)tablero[estado.torreCB[0]][estado.torreCB[1]].ficha);
                reyBlanco.torreL = ((Torre)tablero[estado.torreLB[0]][estado.torreLB[1]].ficha);
                reyNegro.torreC = ((Torre)tablero[estado.torreCN[0]][estado.torreCN[1]].ficha);
                reyNegro.torreL = ((Torre)tablero[estado.torreLN[0]][estado.torreLN[1]].ficha);

                // Actualizamos resto de cosas
                this.actualizarMovimientos();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error cargando estado previo: \n\r"+e.Message);
            }
        }
    }
}
