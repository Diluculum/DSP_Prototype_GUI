   M S B A M L   `   `   `  ����L  HDSP_Prototype_GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:DSP_Prototype_GUIDSP_Prototype_GUI  5   	   "    DSP_Prototype_GUI.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:DSP_Prototype_GUI     5   	     � Title$  DSP Proto Board��5   	   $	��466��6!   $	��566��6.   .��5	      �  � 	Resources 5
      $   DSP_Prototype_GUI.BaudRateList   	baudrates��%   &	          5      6   5      .+�5      �� -   5         ��Name$ openPORT_button��5      $��25��6;   $	��Top=�6G   $
��Left=�6_   $��98��6z   $��
182,99,0,0q�6�   {�.  .��6�   	Open Port6�   �� 5      -   5         ��Name$ portSTA��5   
   $��
291,95,0,0q�6   $��34��6/   $	��Top=�6;   $
��Left=�6S   $	��103��6n   �� 5      -   5        $ sendText��5      $��95,392,333,0q�6   $��23��6J   $	��Top=�6V   .��6n   Send6t   �� 5      -   5        $ 
outputText��5   
   $��244,204,39,0q�6   $��	Output : ��64   $	��182��6H   $	��Top=�6U   $��1,1,1,1q�6m   ���	#FFABADB36�   �� 5      -   5         ��Name$ 	inputText��5       �� TextWrapping$
 Wrap=�6   $��12,204,333,0q�64   $	��183��6J   $	��Top=�6W   .��5      5      �� 5      -   5        $ 	scanPorts��5      $��30��66   $	��Top=�6B   $
��Left=�6Z   $	��138��6u   $��	18,11,0,0q�6�   .��6�   Scan6�   �� 5      -   5         ��Name$ portListGUI��5       �� SelectionMode$ Single=�6!   $��18,46,0,262q�6b   $
��Left=�6w   $	��138��6�   ��5      �� 5      .R�5      � $	��0,1q�6    � ColumnDefinitions 5      �� 5   	   $��* �6   �� 5   	   $	��100 �6   .+�5      �� ��6   �� *
PortName+5      5      5!      �� 5"      -   5        	 ��Name$	 BaudRateBox��5"      #��[  6"   
 �� Text$
 Choose a Baudy��6K    �� 
IsEditable .  �� 
IsReadOnly . $
��Left=�6�   $��
180,46,0,0q�6�   $	��Top=�6�   $	��120��6�   $��25��6�   �� 5#      -	   5        $ BaudRateLabel��5#   
   $��
Baud Rate:��6!   $
��Left=�66   $��
180,20,0,0q�6Q   $	��Top=�6e   $	��100��6}   $��26��6�   5$      5%      