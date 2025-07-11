﻿using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class EditarAnuncioView : Page
    {
        public EditarAnuncioView(MainWindow mainWindow, Imovel imovel, Func<Task> atualizarPainel)
        {
            InitializeComponent();
            DataContext = new EditarAnuncioViewModel(imovel, mainWindow.VoltarParaPainel, atualizarPainel);
        }
    }
}
