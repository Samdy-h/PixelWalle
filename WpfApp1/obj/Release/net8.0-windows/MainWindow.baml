   M S B A M L   `   `   `  ����   WpfApp1    PixelArtEditor.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Pixel Art Editor��$	��600��$	��800��.���  � ColumnDefinitions �� $	��150 ��� $��* �.+��� $��0�����#f0f0f0 �� Padding$ 10q�._��� .+��� $��Tamaño de cuadrícula:��$
��Bold"�$��0,0,0,10q��� -    ��Name$ GridSizeComboBox�� �� SelectedIndex$ 0��$��0,0,0,10q�.@��� 
 � Tag$ 8��.��8x8�� $ 16��.��16x16�� $ 32��.��32x32�� $ 64��.��64x64�� $	 128��.��	128x128�� $	 256��.��	256x256�� $��Tamaño del Canvas:��$
��Bold"�$��0,10,0,5q���  �� Orientation$ 
Horizontal=�$��0,0,0,5q�.+��� $��Ancho:��$��Center=�$��50���� -    ��Name$ CanvasWidthTextBox��$	��512��$��50���� $ 
Horizontal=�$��0,0,0,20q�.+��� $��Alto:��$��Center=�$��50���� -   $ CanvasHeightTextBox��$	��512��$��50���� -   $��Aplicar Tamaño��$
��0,10q�$
��10,5q��� -   $��Borrar todo��$
��0,10q�$
��10,5q��� $��1��$��10q�$
��Auto=�$
��Auto=�.���� -    ��Name$ CanvasBorder��
�������._��� -   	 ��Name$	 DrawingCanvas��
�������