   M S B A M L   `   `   `  ����  
BattleShip0    *BattleShip.View.Controls.GameStatusControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      
������ .��5      �   � ColumnDefinitions  5	   
   �� 5
      $	��185 �6   �� 5      $��* �6   �� 5      $	��185 �6    � 	Resources 5   
    �� 
TargetType �� Property �� Value��%.  '��       '�� �     �� 5      " ��6   .�5      ��  � ��$ Center=�66   �� 5       � ��$ Center=�68   �� 5       �@��Orientation$ 
Horizontal=�60   5      �� 5      " ��6   .�5      ��  � ��
 �����6/   �� 5       � ��$ 18(�6-   5      5      .+�5   
   �� $��0��6   .+�5      �� $��Sunk Ships: ��6   �� 5      ��6   �� *HumanPlayerSunkShips+ �� RelativeSource �� *FindAncestor+ �� AncestorType" >�5      �� 5    
   $��1��6   .+�5!      �� $��Current Game Status: ��6   �� 5"      ��6   �� *CurrentGameStatus+ �� *FindAncestor+" >�5#      �� 5$   
   $��2��6   .+�5%      �� $��Sunk Ships: ��6   �� 5&      ��6   �� *ComputerPlayerSunkShips+ �� *FindAncestor+" >�5'      5(      5)      