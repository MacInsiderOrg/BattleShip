   M S B A M L   `   `   `  ����  
BattleShip)    #BattleShip.View.TwoPlayersGameSetup   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        N� Title$)  #BattleShip - Two players game setup��5      .��5      ]� ��5	   
   �� 5
       �� ImageSource$- '../Resources/images/setupBackground.jpg��6    j� Stretch$
 Fill=�6O   .+�5   
   ]� $	��Top=�6    � 	Resources 5       �� 
TargetType �� Property �� Value��%/  '��       '�� �     �� 5      " ��6   .�5      ��  �@��Orientation$ 
Horizontal=�64   �� 5       � ��$ Center=�6<   �� 5       � ��$ 0 20 0 0q�6/   5      �� 5      " ��6   .�5      ��  � ��$	 5 0q�6/   �� 5       � ��$ 10q�60   5      5      .+�5      �� $
��Left=�6   .+�5      �� -   5        $��80��5      $	��5 0q�6$   $��10q�61   $
��Back��6>   6M   5      �� 5      $
��Left=�6   $	��509��6/   .+�5       �� $��Player Name:��6   $��Center=�5!      $��Center=�65   $��5 0 0 0q�6P   $��16(�5"      $
��Bold"�6&   ���	#FFDADADA68   �� 5#      -   5         ��Name$ PlayerNameTextBox��5#      $	��140��5%      $
��10 0q�6&   $��10q�64    �� 	MaxLength$ 20��6A   ��5$      �� *
PlayerName+	 �� Mode$	 TwoWay=�5%   A   �� 5&      -   5        $��
Start Game��5&      $	��140��6N   5'      5(      � 5*   
   $��	5 20 5 10q�6    5+      
 �� 	Resources �� Color �� Opacity��%�  '��       '�� 
    '�� �    '�� 0    �� 5,      " ��6   
 5-      ���� 5/      $ DarkCyan��6\   $ .8Q�6m   (��       6*   * SystemColors.HighlightBrushKey+)6m   �� 52      $ DarkCyan��6Z   $ .8Q�6k   (��       6*   *SystemColors.ControlBrushKey+)6k   53      54      �� 56      " ��6   .�57      ��  � ��$ Center=�6<   �� 58       � ��$ 26(�61   �� 59       � ��$
 Bold"�63   5:      �� 5<      " ��6   .�5=      ��  � ��
 �   �64   �� 5>       � ��$ 4q�68   �� 5?       � ��$ 3q�6/   5@      �� 5B      " ��6   .�5C      ��  � ��
 ���� 63   5D      5E      .+�5G      ]� .+�5H      �� $��CHOOSE YOUR AVATAR��6   $	��Top=�65   ���	#FFDADADA6J   �� 5I      ._�5J      �� -   5         ��Name$ ChoosePlayerAvatar��5J      ��5K      �� *HumanPlayerAvatar+$	 TwoWay=� �� SelectedItem 5L      �� *HumanPlayerAvatarItem+$	 TwoWay=���5M      �� 5N      .R�5O   "   �  � 
ItemHeight$	 130��6�   ��6,   �� * (FrameworkElement.ActualWidth)+ �� RelativeSource ��  �� AncestorType$ ScrollContentPresenterL�6�   5P      5R      5S      5T      5U      5V      5W      