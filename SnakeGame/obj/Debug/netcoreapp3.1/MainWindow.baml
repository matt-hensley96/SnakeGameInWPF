   M S B A M L   `   `   `  ����D  @SnakeGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:SnakeGame	SnakeGame  5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R8clr-namespace:System.ComponentModel;assembly=WindowsBaseSystem.ComponentModel 5	   	       SnakeGame.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:SnakeGame     5   	   Hscm8clr-namespace:System.ComponentModel;assembly=WindowsBase     5	   	   -   5          �Name$  Window��5   	    � Title$ Snake - Score 0��5
   	    � SizeToContent$ WidthAndHeight=�6!    � 
ResizeMode$ NoResize=�5   	    � WindowStyle$
 None=�6   
���   � � 	Resources 5         HighScoreListViewSource �� Source �� ElementName �� Path	 �� SortDescriptions+  %System.ComponentModel.SortDescription
   	Direction   PropertyName��%�   &	        �� 5   
    6   �� $ Window��$ 
HighScores�	 5        5      $
 
Descending=�6&   $ Score��6=   5      5      .��5      ]� 
���   �.+�5   
   � -   5        $��PanelTitleBar��5   $   $	��Top=�6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $
��Auto �6#    5       �� 
TargetType �� Property �� Value��%�   '��       �� 5      " ��6   .�5      ��  � ��$ Consolas)�63   �� 5        � ��
 �����63   �� 5!       � ��$ 24(�61   �� 5"       � ��$
 Bold"�63   5#      5$      .+�5&      � $��10,0,0,0q�6   .+�5'      �� Score:6$   �� 5(      -   5        $��TextBlockScoreValue��5(      06:   5)      � 5*      $��1��6   .+�5+      �� Speed:6$   �� 5,      -   5        $��TextBlockSpeedValue��5,      06:   5-      �� 5.      -   5        $��BtnClose��5.   �   $��2��6   $��Right=�6%   
������ 
�������$
��Bold"�6h   $��20(�6z   $��0q�6�   $
��10,0q�6�   .��6�   X6�   5/      �� 51   
   
���   �$��5q�6%   ._�52      �� -   5        $��GameArea��52      $
}�True��6%   $	��400��69   $	��400��6E   .+�53      �� -   5        $��BorderWelcomeMessage��53   |   
�������$��2q�6.   $	��300��6B   $	��300��6N    �� Left$ 50��6[   
 �� Top$ 50��6l    K� ZIndex$ 1��6�   ._�54      ��  �� Orientation$ Vertical=�6!   
�������.+�55      �� $
��Bold"�6$   $��50(�66   $��Center=�6D   $��0,20,0,0q�6a   Snake6z   �� 56      $
��Bold"�6$   $��Center=�66   $��24(�6S   
���  ��Press SPACE to start!6�   �� 57      -   5        $��BtnShowHighScoreList��57   -   $��20q�6!   $��Center=�6l   $
��10,3q�6�   .��6�   Show High Score List...6�   58      59      5;      5<      �� 5>   
   -	   5        $��BorderHighScoreList��5>   t   
�������$��2q�6&   $	��300��6:   $	��300��6F   $ 50��6S   $ 50��6d   $ 1��6�   $y�	Collapsed=�6�   ._�5?      �� $ Vertical=�6   
�������.+�5@      �� $��0,0,0,2q�6   
�������$
��0,10q�6H   ._�5A      �� $��Center=�6    $��34(�6=   $
��Bold"�6K   High Score List6n   5B      �� 5C      ��6   ��  �� Source# [  ��5D      �� 5E      .R�5F      ]� $��7q�6(   .+�5G   "   �� $
��Left=�6H   $��22(�6^   ��6,   �� *
PlayerName+6n   �� 5H   "   $��Right=�6C   $��22(�6Z   $��Right=�6h   ��6,   �� *Score+6�   5I      5J      5L      5M      5N      �� 5P   
   -
   5        $��BorderNewHighScore��5P   t   
�������$��2q�6&   $	��300��6:   $	��300��6F   $ 50��6S   $ 50��6d   $ 1��6�   $y�	Collapsed=�6�   ._�5Q      �� $ Vertical=�6   
�������.+�5R      �� $��Center=�6   $��34(�69   $
��Bold"�6G   $��20q�6Y   New High Score!6v   �� 5S      $��Center=�6   $
��Wrap=�69   $��16(�6M   YWCongratulations - you made it into the high score list! Please enter your name below...5U      �� 5V      -   5        $��TextBoxPlayerName��5V      $��28(�63   $
��Bold"�6A    �� 	MaxLength$ 8��6S   $��20q�6a   $��Center=�6m   6�   �� 5W      -   5        $��BtnAddToHighScoreList��5W      $��16(�66   $��Center=�6D   $
��10,5q�6�   .��6�   Add to high score list6�   5X      5Y      �� 5[   
   -   5        $��BorderEndOfGame��5[   t   
�������$��2q�6&   $	��300��6:   $	��300��6F   $ 50��6S   $ 50��6d   $ 1��6�   $y�	Collapsed=�6�   ._�5\      �� $ Vertical=�6   
�������.+�5]      �� $��Center=�6   $��40(�69   $
��Bold"�6G   $��0,20,0,0q�6Y   Uh oh!6s   �� 5^      $��Center=�6   $��26(�69   $
��Bold"�6G   ...you died!6g   �� 5_      $��20q�6    �� TextAlignment$ Center=�6(   $��16(�6?   Your score:6Z   �� 5`      -   5        $��TextBlockFinalScore��5`      $ Center=�67   $��48(�6N   $
��Bold"�6\   
���  ��06�   �� 5a      $ Center=�6   $��16(�63   $��20q�6A   " Press SPACE to start a new game!6o   5b      5c      5d      5e      