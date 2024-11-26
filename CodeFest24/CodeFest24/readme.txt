	API-1a	
	
	POST
	join game											
	METHOD																																		Emmited request to sign your bot to game.																						
	socketClient.emit('join game', Parameters)																																		 It is required.																						
																																																																														
	INPUT PARAMETERS																																		Example in JavaScript:																						
		- game_id:				 The verification key of a match. Data type is String.																													socketClient.emit('join game', { game_id: 'xxxx-xxxx-xxxx-xxxx',																						
							Admin will share to you before every fighting time.																																					 player_id: 'xxxx-xxxx-xxxx-xxxx'});														
																																																																														
		- player_id: 				The verification key of team.  Data type is String.																																																			
						Admin will share to you before every fighting time																																																			
																																																																														
	Data Format:				JSON																																																				
																																																																														
	Data Structure:																																																								
		- game_id:				The key of game.																																																			
		- player_id:				The player key.																																																			
																																																																														
	API-1b
	POST	
	join game	
	METHOD		
	Example:																						
	socketClient.on('join game', callbackFunction)																																			{																					
																																					game_id:"a7557424-be4f-47e9-b20e-345b055bf128",																				
	RESPONSE PARAMETERS																																				player_id:"player1-xxx"																				
		Response parameters of callbackFuntion(Response)																																		}																					
																																																																														
		Data Format:				JSON																																																			
																																																																														
		Data Structure:																																																							
			- game_id:				The key of game.																																																		
			- player_id:				The player key. We will simplify it to the first 13 letters of the player key																																																		
																																																																														
	API-2
	POST
	ticktack player		
	METHOD			
	Return a ticktack every time game updates an event		
	
socketClient.on('ticktack player', callbackFunction)																																																								
																																																																														
RESPONSE PARAMETERS																																		It is a required implementation for tracking game states.																						
Response parameters of callbackFuntion(Response)																																																							
																																	The event provides information about the game, including the map 																						
Data Format:				JSON																													size, player states, current map states, spoils, bombs, etc.																						
																																																																														
Data Structure:																																	Here is the example of  Training Game's ticktack:																						
	- id				ID of request																													{																					
	- timestamp				Time of request																														"id": 15,																				
	- map_info				Map information																														"timestamp": 1730277449860,																				
					Properties of Map Info																														"map_info": {																				
						- size: 			The information of map. 																											"size": { "cols": 26, "rows": 14 },																			
									Include size of map by horizotal and vertical squares.																											"players": [																			
									Properties of Size:																												{																		
										- rows:			The max number of cell of map by vertical.																									"id": "player1-xxx",																	
										- cols:			The max number of cell of map by horizontal.																									"currentPosition": { "col": 22, "row": 8 },																	
						- players: 			The array list of player on map.																													"spawnBegin": { "col": 22, "row": 8 },																	
									Properties of Player:																													"score": 0,																	
									- id :						The Id of competitor or player																							"lives": 1000,																	
									- spawnBegin: 						The player or conpetitor's position is born at the start of the																							"transformType": 1,																	
															game on map  (row,col) by horizotal and vertical squares.																							"ownerWeapon": [1],																	
									- currentPosition:						The current position of player or competitor on map (row,col) 																							"currentWeapon": 1,																	
															by horizotal and vertical squares.																							"hasTransform": false,																	
									- power: 						The power of the player's bomb (not of the competitor).																							"timeToUseSpecialWeapons": 5,																	
									- speed: 						The speed of the player (not of the competitor).																							"isStun": false,																	
									- delay: 						Time interval between 2 times of placing a bomb of the 																							"speed": 230,																	
															player(not of the competitor).																							"power": 1,																	
									- score:						Current score of player.																							"delay": 2000,																	
									- lives:						Player’s lives left.																							"box": 0,																	
									- box: 						Number of destroyed box.																							"stickyRice": 0,																	
									- stickyRice:						Number of items collected																							"chungCake": 0,																	
									- chungCake:						Number of items collected																							"nineTuskElephant": 0,																	
									- nineTuskElephant:						Number of items collected																							"nineSpurRooster": 0,																	
									- nineSpurRooster:						Number of items collected																							"nineManeHairHorse": 0,																	
									- nineManeHairHorse:						Number of items collected																							"holySpiritStone": 0,																	
									- holySpiritStone:						Number of items collected																							"eternalBadge": 0,																	
															This item will help the player increase the number of uses																							"brickWall": 0																	
															of the special weapon of the god they have chosen.																						},{																		
															Max: 4 times																							"id": "player2-xxx",																	
									- eternalBadge:						Received when the player collects 5 required items.																							"currentPosition": { "col": 3, "row": 7 },																	
															After owning this badge, he will be able to marry Mi Nuong																							"spawnBegin": { "col": 3, "row": 7 },																	
									- brickWall:						Number of brick walls you destroyed with the wooden pestle																							"score": 0,																	
									- transformType:						The kind of god you want to be																							"lives": 1000,																	
									- hasTransform:						Tell if you have become a god or not																							"transformType": 1,																	
									- ownerWeapon:						Weapons you own																							"ownerWeapon": [1],																	
															Values of weapon in array:																							"currentWeapon": 1,																	
															1:	Wooden pestle																						"hasTransform": false,																	
															2:	Phach Than (Bomb)																						"timeToUseSpecialWeapons": 5,																	
									- curWeapon:						The weapon the player is using																							"isStun": false,																	
									- isStun:						Indicates whether the player is stunned or not.																							"speed": 230,																	
															(stunned by weapon: wooden pestle, lasts 3 seconds)																							"power": 1,																	
									- isChild:						Check the player is child or not																							"delay": 2000,																	
									- timeToUseSpecialWeapons:							Number of times special weapons can be used remaining.																						"box": 0,																	
																																						"stickyRice": 0,																	
						- map: 			2D array describing infomation of map.																													"chungCake": 0,																	
									Values of item in array:																													"nineTuskElephant": 0,																	
											0 - Empty Cell (Can move through them)																											"nineSpurRooster": 0,																	
											1 - A Wall (None destructible cell)																											"nineManeHairHorse": 0,																	
											2 - A Balk (Destructible cell)																											"holySpiritStone": 0,																	
											3 - A Brick Wall (Destructible cell by wooden pestle weapon)																											"eternalBadge": 0,																	
											5 - A Prison Place 																											"brickWall": 0																	
											6 - God Badge (used to turn the character into a God)																										}																		
											7 - Cells destroyed by special weapons (Can move through them)																									],																			
						- bombs:			The array list of bombs on map.																											"map": [																			
									Properties of Bomb:																												[ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ],																		
										- row:				The horizontal position of the item.																							[ 1, 5, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 5, 1 ],																		
										- col:				The vertical position of the item.																							[ 1, 2, 0, 0, 0, 0, 0, 0, 0, 1, 0, 2, 2, 2, 2, 1, 1, 1, 1, 0, 0, 0, 0, 0, 2, 1 ],																		
										- remainTime: 				The bomb will explode at [specific time]																							[ 1, 2, 2, 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 2, 2, 1, 0, 0, 0, 1, 0, 0, 0, 2, 2, 1 ],																		
										- playerId:				Owner of bomb																							[ 1, 0, 0, 2, 0, 0, 0, 1, 0, 1, 0, 2, 2, 2, 3, 3, 3, 3, 0, 1, 0, 0, 2, 0, 0, 1 ],																		
										- power:				When was the bomb created?																							[ 1, 6, 0, 2, 2, 0, 0, 0, 0, 1, 0, 2, 2, 2, 0, 1, 0, 0, 0, 1, 0, 2, 2, 0, 6, 1 ],																		
										- createdAt:				Current bomb power																							[ 1, 2, 0, 0, 2, 0, 0, 0, 0, 1, 0, 2, 2, 2, 0, 1, 1, 1, 1, 0, 0, 2, 0, 0, 2, 1 ],																		
						- spoils: 			The array list of spoils on map.																												[ 1, 2, 0, 0, 2, 0, 0, 0, 0, 1, 0, 2, 0, 0, 0, 1, 0, 3, 0, 1, 0, 2, 0, 0, 2, 1 ],																		
									Properties of Spoil:																												[ 1, 2, 0, 0, 2, 0, 0, 0, 0, 1, 0, 2, 2, 0, 3, 3, 3, 3, 0, 1, 0, 2, 0, 0, 2, 1 ],																		
										- row:				The position of item in horizontal.																							[ 1, 0, 0, 2, 2, 0, 0, 0, 0, 1, 0, 0, 2, 2, 0, 1, 0, 0, 0, 1, 0, 2, 2, 0, 0, 1 ],																		
										- col:				The position of item in vertical.																							[ 1, 0, 0, 2, 0, 0, 0, 1, 1, 1, 1, 1, 0, 2, 2, 1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 1 ],																		
										- spoil_type : 				Spoil type specification																							[ 1, 2, 2, 0, 0, 0, 0, 3, 2, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 0, 0, 0, 2, 2, 1 ],																		
														Values of spoil types: 								Bonus:															[ 1, 5, 0, 0, 0, 0, 0, 2, 2, 2, 0, 6, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 5, 1 ],																		
														32 - STICKY RICE								1 score															[ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ]																		
														33 - Chung Cake								2 score														],																			
														34 - Nine Tusk Elephant								5 points (increases power by 1 unit)														"bombs": [],																			
														35 - Nine Spur Rooster								3 score														"spoils": [],																			
														36 - Nine Mane Hair Horse								4 score														"weaponHammers": [],																			
														37 - Holy Spirit Stone								3 score														"gameStatus": null,																			
									If you collect 5 items of each type (32-36), you will increase the bomb's power by 1 unit.																											"cellSize": 35																			
						- weaponHammers:						The array list of hammer on map.																							},																				
												Properties of hammers:																							"tag": "update-data",																				
													- playerId:				playerId of the hammer's owner																		"gameRemainTime": 0																				
													- power:				blast radius after hammer hits target																	}																					
													- destination:				hammer destination																																						
													- createdAt:				the moment the hammer was created																																						
																																																																														
						- weaponWinds						The array list of weapon wind on map.																																											
												Properties of weaponWinds:																																											
													- playerId:				playerId of the hammer's owner																																						
													- currentRow:				hammer row position																																						
													- currentCol:				hammer col position																																						
													- createdAt:				the moment the weapon wind was created																																						
													- destination:				weapon wind destination																																						
																																																																														
						- cellSize:			The size of a cell on the map																																														
									Values of cell:																																														
										- Training: 				35x35 pixel																																									
										- Fighting:				55x55 pixel																																									
						- gameStatus: 				Only have in fighting stage.																																													
										running round:								2																																					
										pause round:								3																																					
										game over:								10																																					
	- tag: 		The reason for this ticktac update																																																				
			Player's tag values:																																																				
					player:moving-banned								Player's moving is blocking, two player moves to same cell.																																										
					player:start-moving								Player is started moving.																																										
					player:stop-moving								Player is stopped moving.																																										
					player:be-isolated								Player is blocking in quarantine area.																																										
					player:back-to-playground								Player was moved out quarantine area, and stand on floor.																																										
					player:pick-spoil								Player just picked a spoil.																																										
					player:stun-by-weapon								Notice when the player is stunned																																										
					player:stun-timeout								Notifies the player that the stun time has expired.																																										
			Bomb's tag values:																																																				
					bomb:exploded								Bomb has exploded.																																										
					bomb:setup								Bomb has setup on map.																																										
			Game's tag values																																																				
					start-game								Game is started.																																										
					update-data								Auto looping in interval time. (Ex: every 500ms)																																										
			Wedding's tag values:																																																				
					player:into-wedding-room								Notice that the player go to the wedding room																																										
					player:outto-wedding-room								Notice that the player leaving the wedding room																																										
					player:completed wedding								Declare that the marriage is complete																																										
			Hammer tag values																																																				
					hammer:exploded								Location where the hammer hits																																										
			Wooden-pestle tag values																																																				
					Wooden-pestle:setup								Player starts to use pastle																																										
			WeaponWind tag values																																																				
					wind:exploded								Current location of the weapon wind (in horizontal and vertical)																																										
																																																																														
	- player_id: 				The player’s id whose action made the update 																																																		
	- gameRemainTime:						Remain time in seconds of game before time over.																																																
																																																																														
	API-3
	POST
	drive player
	METHOD																																		Send request to move your bot																						
			socketClient.emit('drive player', Parameters)																																																								
																																					Example-1: Move player with many steps																						
			INPUT PARAMETERS																																		socketClient.emit('drive player', {																						
				- direction:				The player's series of movement direction. Data type is String.																													    "direction": "1111333332222224444",																						
								Value of direction:																													})																						
										1 - Move LEFT																																																	
										2 - Move RIGHT.																											Example-2: Move bomber with only one step																						
										3 - Move UP																											socketClient.emit('drive player', {																						
										4 - Move DOWN																											    "direction": "3",																						
										b 	Based on the current weapon used to activate:																										})																						
													- If the current weapon is a wooden bat. will perform a bat strike 																																															
													in the direction of the character's face																								Example-3: Move player and use the weapon																						
													- If the current weapon is a Phach Than. will perform a lightning strike																									socketClient.emit('drive player', {																						
													at the current location																								    "direction": "111b2222",																						
										x - Stop Moving																											})																						
					- characterType:							The player decides who they want to control.																																																
											If they want to control a `Supporting Character`. characterType is 'child'																										Example-4: Move `supporting character` with many steps																						
																																					socketClient.emit('drive player', {																						
											Supporting Character: This character will appear after the player marries Mi Nuong.																										    "direction": "1111333332222224444",																						
			Data Format:				 JSON																														    "characterType": "child",																						
																																					})																						
			Data Structure:																																																								
							{																														Example-5: Move `supporting character` with only one step																						
								"direction": "xxxxx"																														socketClient.emit('drive player', {																						
								"characterType": "child" | undefined																														    "direction": "2",																						
							}																														    "characterType": "child",																						
																																					})																						
																																																																														
																																					Example-6: Move `supporting character`  and use the weapon																						
																																					socketClient.emit('drive player', {																						
																																						"direction": "111b2222",																						
																																						"characterType": "child",																						
																																					})																						
																																																																														
	API-4				POST
	register character power
	METHOD					
	
		socketClient.emit('register character power', Parameters)																																		Example:																						
																																				socketClient.emit('register character power', {																						
		INPUT PARAMETERS																																		    "gameId": "a7557424-be4f-47e9-b20e-345b055bf128",																						
				- gameId: 						 The verification key of a match. Data type is String.																											    "type": 1,																						
										Admin will share to you before every fighting time.																											})																						
																																																																														
				- type:						This is the step where players will sign up to see which god they will become in the game.																																																	
									Data type is String.																																																	
									Value of type:																																																	
										1 - 		` Mountain God `																																														
										2 - 		` Sea God `																																														
			Data Format:				JSON																																																			
																																																																														
			Data Structure:																																																							
					- direction:				The player's movement direction.																																																	
					- player_id:				The moved player																																																	
																																																																														
			Note: 		If the player does not register which god they want to transform into, at the time the game starts,																																																					
							we will randomly choose.																																																				
	API-5				POST	
	actions											
	METHOD																																																								
	socketClient.emit('action', Parameters)																																		Example -1a: Action 'switch weapon'																						
																																			socketClient.emit('action', {																						
	INPUT PARAMETERS																																			"action": "switch weapon"																					
		- action:						Action you want to take. Data type is String																											})																						
								Values of action:																																																	
									switch weapon:					Change weapon to other than current weapon.																					Example -1b: Action 'switch weapon' of child																						
									use weapon:					Allows you to use special weapons corresponding to the gods you have chosen.																					socketClient.emit('action', {																						
														If you are the `Mountain God` you will need to send one more parameter																						"action": "switch weapon"																					
														called payload to specify the destination of the special weapon.																						"characterType": "child"																					
									marry wife:					When you collect enough items and can marry Mi Nuong, You will use																					})																						
														this aciton to marry Mi Nuong																																											
																																			Example -2a: Action 'use weapon'																						
		- payload:						This second parameter will allow you to send to the specific destination where you want to use																											socketClient.emit('action', {																						
								the special weapon. Data type is Object																												"action": "use weapon",																					
								Values of payload:																												"payload": {																					
									- destination:					{																							"destination": {																				
															"col":		number																					"col": 13																			
															"row":		number																					"row": 4																			
														}																							}																				
		- characterType:						If you want to control a sub-character (child) to perform these actions. Data type is String																												}																					
								Values of characterType:																											})																						
									child																																																
																																			Example -2b: Action 'use weapon' of child																						
																																			socketClient.emit('action', {																						
																																				"action": "use weapon",																					
																																				"payload": {																					
																																					"destination": {																				
																																						"col": 13																			
																																																											"row": 4																			
																																																										}																				
																																																									}																					
																																																									"characterType": "child"																					
																																																								})																						
																																																																														
																																																								Example -3: Action 'marry wife'																						
																																																								socketClient.emit('action', {																						
																																																									"action": "marry wife"																					
																																																								})																						
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														
																																																																														