   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:PixelWallE.ViewsPixelWallE.Views  5   	   9!clr-namespace:PixelWallE.ControlsPixelWallE.Controls  5   	   !    PixelWallE.Views.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   0localclr-namespace:PixelWallE.Views     5   	   6controls!clr-namespace:PixelWallE.Controls     5   	     � Title$  Pixel Wall-E��5	   	   $	��700��6   $
��1000��6+   .��5
      �  � RowDefinitions 5      �� 5      $��* �6   �� 5      $
��Auto �6   .+�5      � $��0��6	    � ColumnDefinitions 5      �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $	��300 �6   .+�5      ]� $��0��6   $��5q�6   .+�5      �� $	��Top=�6   $��Editor de Codigo��6%   $
��Bold"�6=   $��0,0,0,5q�6O   � 5       5      �� 5      $
��Auto �6   �� 5      $��* �6   .+�5"      �� $��0��6   
�������$��0,0,1,0q�66   ._�5#      �� ��6   �� *LineNumbers+��5$   	   �� 5%   
   .R�5&      �� $��30��5'   0    �� TextAlignment$ Right=�5(   0    �� Padding$ 0,2,5,2q�5)   0   
���������5&      �� 5*   0   5+      5-   	   5.      �� 51      -   5        $��1��51      $��Consolas)�53      $��14(�54       �� AcceptsReturn$
 True��55      " �� VerticalScrollBarVisibility$
 Auto=�56      $ �� HorizontalScrollBarVisibility$
 Auto=�57      ��52      �� *Code+ �� UpdateSourceTrigger$ PropertyChanged=�58      59      5:      �� 5=      $��1��6   $��5��6"   $��Center=�6,   
�������6I   � 5@      $��2��6
    5A      �� 5B      $
��Auto �6   �� 5C      $��* �6   �� 5D      $
��Auto �6   �� 5E      $
��Auto �6   .+�5J      �� $��0��6   $��Estado de Wall-E��6   $��5q�66   .��5K      �� .+�5L      �� $��5q�68   ��6   �� *WallEPositionDisplay+68   �� 5M      $��5q�67   ��6   �� *CurrentColorDisplay+67   �� 5N      $��5q�64   ��6   �� *BrushSizeDisplay+64   �� 5O      $��5q�65   ��6   �� *CanvasSizeDisplay+65   5P      5Q      �� 5T      $��1��6   $��Canvas��6   $��5q�6,   .��5U      '   !PixelWallE.Controls.CanvasControl  	 � DataContext	 6   �� 5V      �� 5Y      $��2��6   $��
Resultados��6   $��5q�60   .��5Z      �� $
��Auto=�6   $	��150��67   .��5[      �� $
��Wrap=�5\   $   $��5q�5]   $   ��5[      �� *ExecutionResult+5]   $   5^      5_      �� 5b      $��3��6   $��Errores��6   $��5q�6-   .��5c      �� ���	#FFF0F0F0$��0q�5e      ��5c      �� *Errors+��5f      �� 5g   	   .R�5h   
   �� 
 �� Orientation$
 
Horizontal=�6   $��2q�6.   .+�5i      �� $��Línea ��6   $
��Bold"�6#   �� 5j      $
��Bold"�61   
���  ����6   �� *
LineNumber+6C   �� 5k      $��: ��6   �� 5l      $
��Wrap=�6.   ��6   �� *	Message+6.   5m      5n   
   5p      5q      5r      5s      �� 5v      $��1��6   ���	#FFE0E0E0 �� Padding$ 5q�6/   ._�5w      �� $
 
Horizontal=�6   $��Center=�6)   .+�5x      �� $��Ejecutar (F5)��6   $	��120��5y      $��30��6%   $��5q�61   $
��Bold"�6<   ��5x   %   �� *ExecuteCommand+5y   <   �� 5{      $��Tamaño Canvas:��6   $��Center=�6&   $��10,0,5,0q�6A   �� 5|      $��50��5}      $��25��6%   $��Center=�61   ��5|      �� *
CanvasSize+$ PropertyChanged=�5}   1   �� 5      $��Redimensionar��6   $	��100��5�      $��30��6%   $��5q�61   ��5   %   �� *ResizeCanvasCommand+5�   1   �� 5�      $��Cargar Archivo��6   $	��100��5�      $��30��6%   $��5q�61   ��5�   &   �� *LoadCommand+5�   1   �� 5�      $��Guardar Archivo��6   $	��100��5�      $��30��6%   $��5q�61   ��5�   '   �� *SaveCommand+5�   1   5�      5�      5�      5�      