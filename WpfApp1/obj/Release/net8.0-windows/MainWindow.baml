   M S B A M L   `   `   `  ����   WpfApp1    PixelArtEditor.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Pixel Art Editor��$	��600��$	��900��.���  � ColumnDefinitions �� $	��200 ��� $��* �.+�� $��0�� � RowDefinitions �� $��* ��� $
��Auto �.+��� $��0��
�������$��1q�$��10q�
�������._��� -    ��Name$ ConsoleScrollViewer��.���� -    ��Name$ TextDisplay��$��5q�$
��Wrap=�
���   �$��Consolas)��� $��1��$��
10,0,10,10q�.+��� -    ��Name$ 	TextInput��$��60�� �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=� �� AcceptsReturn$
 True��$��Bottom=�$��Consolas)�
����������	#FF333333	 �� 
CaretBrush
	 ������� -   $��Ejecutar��$��0,5,0,0q�$��5q����	#FF555555
�������
 �� 	IsDefault$
 False�� �� IsCancel$ False��� $��1�� �� $
��Auto ��� $��* �.+��� $��0�����#f0f0f0 �� Padding$ 10q�._���  �� Orientation$ 
Horizontal=�.+��� -   $��Tamaño��$
��10,5q�$��0,0,10,0q����	#FF555555
��������� -   $��Guardar��$
��10,5q�$��0,0,10,0q����	#FF555555
��������� -   $��Abrir��$
��10,5q�$��0,0,10,0q����	#FF555555
��������� -   $��Borrar��$
��10,5q����	#FF555555
��������� $��1��$��10q�$
��Auto=�$
��Auto=�.���� -	    ��Name$ CanvasBorder��
�������$	��512��$	��512��._��� -
    ��Name$ DrawingCanvas��
�������$	��512��$	��512��