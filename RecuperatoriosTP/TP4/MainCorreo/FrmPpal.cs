﻿//using Entidades;
using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainCorreo
{
    public partial class FrmPpal : Form
    {
        private Correo correo;

        public FrmPpal()
        {
            this.InitializeComponent();
            this.correo = new Correo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Paquete paquete = new Paquete(txtDireccion.Text, mtxtTrackingID.Text);
            paquete.InformaEstado += new Paquete.DelegadoEstado(this.paq_InformaEstado);
            try
            {
                this.correo += paquete;
            }
            catch (TrackingIdRepetidoException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.ActualizarEstados();
        }


        private void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            this.MostrarInformacion<List<Paquete>>((IMostrar<List<Paquete>>)this.correo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elemento"></param>
        private void MostrarInformacion<T>(IMostrar<T> elemento)
        {
            if(!(elemento is null))
            {
                this.rtbMostrar.Text = elemento.MostrarDatos(elemento);
                if(elemento is Correo)
                {
                    try
                    {
                        this.rtbMostrar.Text.Guardar("salida.txt");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }                
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        private void ActualizarEstados()
        {
            lstEstadoIngresado.Items.Clear();
            lstEstadoEnViaje.Items.Clear();
            lstEstadoEntregado.Items.Clear();
            foreach (Paquete paquete in correo.Paquetes)
            {
                switch (paquete.Estado)
                {
                    case Paquete.EEstado.Ingresado:
                        this.lstEstadoIngresado.Items.Add(paquete);
                        break;
                    case Paquete.EEstado.EnViaje:
                        this.lstEstadoEnViaje.Items.Add(paquete);
                        break;
                    case Paquete.EEstado.Entregado:
                        this.lstEstadoEntregado.Items.Add(paquete);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paq_InformaEstado(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Paquete.DelegadoEstado d = new Paquete.DelegadoEstado(paq_InformaEstado);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                this.ActualizarEstados();
            }
        }

        ///// <summary>
        ///// Chequea si hay hilos vivos, de ser así los termina
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private void FrmPpal_FormClosing(object sender, FormClosingEventArgs e)
        {
            correo.FinEntregas();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mostrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MostrarInformacion<Paquete>((IMostrar<Paquete>)lstEstadoEntregado.SelectedItem);
        }
    }
}
