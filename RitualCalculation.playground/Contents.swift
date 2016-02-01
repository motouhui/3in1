//: Playground - noun: a place where people can play

import Foundation

//class Fuck {
//    func everySecond(){
//        print("print")
//    }
//}
//
//let fuck = Fuck()

//let t = NSTimer.scheduledTimerWithTimeInterval(1, target: fuck, selector: "everySecond", userInfo: nil, repeats: true)
//t.fire()

let HP_MAX:Float = 100 // HP 最大值
let MP_MAX:Float = 30 // MP 最大值
let HERO_ATTACK:Float = 20 // 英雄攻击力
let BOSS_ATTACK:Float = 15 // 魔王攻击力
let SWORD_DAMAGE:Float = 10 // 剑受到的反伤

let MP_CONSUMPTION:Float = 10 // MP 消费

let CRITICAL_MULTIPLY: Float = 1.2
let DEFENSE_MULTIPLY: Float = 0.8
let CRITICAL_HAPPEN_RATE: Float = 0.1
let HP_RECOVER_RATE: Float = 0.1
let MP_RECOVER_RATE: Float = 0.5

class Hero {
    var currentAction: String = "idle"
    var Hp: Float = HP_MAX
    var Mp: Float = MP_MAX

    func reset() {
        Hp += (HP_RECOVER_RATE * BOSS_ATTACK)
        Mp += (MP_RECOVER_RATE * MP_CONSUMPTION)
        Hp = Hp > HP_MAX ? HP_MAX : Hp
        Mp = Mp > MP_MAX ? MP_MAX : Mp
        currentAction = "idle"
    }

    func attack(boss: Boss, sword: Sword, power: Float = 1) {
        currentAction = "attack"

        var heroRate: Float = power
        var swordRate: Float = power
        if (boss.currentAction == "defend") {
            heroRate = 0.8
            swordRate = 1.2
        }
        if (Float(arc4random_uniform(100))/100.0 < CRITICAL_HAPPEN_RATE) {
            heroRate *= CRITICAL_MULTIPLY
            swordRate *= CRITICAL_MULTIPLY
        }
        Mp -= MP_CONSUMPTION

        if (sword.currentAction != "dodge") {
            boss.Hp -= (HERO_ATTACK * heroRate)
            sword.Hp -= (SWORD_DAMAGE * swordRate)
        }
    }

    func criticalAttack(boss: Boss, sword: Sword) {


        Hp -= 10 // 必会心伤血
        attack(boss, sword: sword, power: CRITICAL_MULTIPLY)
        currentAction = "criticalAttack"
    }

    func pass() {
        currentAction = "pass"
    }

    func showStates() {
        print("HERO: \(currentAction)")
        print(" HP:\(Hp)/\(HP_MAX)")
        print(" MP:\(Mp)/\(MP_MAX)")
    }
}

class Boss {
    var currentAction: String = "idle"
    var Hp: Float = HP_MAX
    var Mp: Float = MP_MAX

    func reset() {
        Hp += (HP_RECOVER_RATE * HERO_ATTACK)
        Mp += (MP_RECOVER_RATE * MP_CONSUMPTION)
        Hp = Hp > HP_MAX ? HP_MAX : Hp
        Mp = Mp > MP_MAX ? MP_MAX : Mp
        currentAction = "idle"
    }

    func attack(hero: Hero) {
        currentAction = "attack"

        Mp -= MP_CONSUMPTION

        var rate: Float = 1
        if (Float(arc4random_uniform(100))/100.0 < CRITICAL_HAPPEN_RATE) {
            rate = CRITICAL_MULTIPLY
        }
        hero.Hp -= (BOSS_ATTACK * rate)
    }

    func defend() {
        currentAction = "defend"
    }

    func pass() {
        currentAction = "pass"
    }

    func showStates() {
        print("BOSS: \(currentAction)")
        print(" HP:\(Hp)/\(HP_MAX)")
        print(" MP:\(Mp)/\(MP_MAX)")
    }
}

class Sword {
    var currentAction: String = "idle"
    var Hp: Float = HP_MAX
    var Mp: Float = MP_MAX

    func reset() {
        Hp += (HP_RECOVER_RATE * SWORD_DAMAGE)
        Mp += (MP_RECOVER_RATE * MP_CONSUMPTION)
        Hp = Hp > HP_MAX ? HP_MAX : Hp
        Mp = Mp > MP_MAX ? MP_MAX : Mp
        currentAction = "idle"
    }

    func dodge(boss: Boss) {
        currentAction = "dodge"

        Mp -= MP_CONSUMPTION
        if (boss.currentAction == "defend") {
            Hp -= SWORD_DAMAGE
        }
    }

    func pass() {
        currentAction = "pass"
    }

    func showStates() {
        print("SWORD: \(currentAction)")
        print(" HP:\(Hp)/\(HP_MAX)")
        print(" MP:\(Mp)/\(MP_MAX)")
    }
}

class Game {
    var state: String = "running"

    var hero: Hero = Hero()
    var boss: Boss = Boss()
    var sword: Sword = Sword()

    func proceed() -> Bool {
        if (hero.currentAction == "idle"
            || boss.currentAction == "idle"
            || sword.currentAction == "idle") {
                print(hero.currentAction)
                print(boss.currentAction)
                print(sword.currentAction)
            print("!!! somebody is idle")
                return false
        }

        switch (boss.currentAction) {
        case "attack":
            boss.attack(hero)
        case "defend":
            boss.defend()
        default:
            boss.pass()
        }

        switch (sword.currentAction) {
        case "dodge":
            sword.dodge(boss)
        default:
            sword.pass()
        }

        switch (hero.currentAction) {
        case "attack":
            hero.attack(boss, sword: sword)
        case "criticalAttack":
            hero.criticalAttack(boss, sword: sword)
        default:
            hero.pass()
        }

        if (hero.Hp <= 0
            || boss.Hp <= 0
            || sword.Hp <= 0) {
            state = "over"
        }

        return true
    }

    func showStates() {
        hero.showStates()
        boss.showStates()
        sword.showStates()
    }

    func reset() {
        hero.reset()
        boss.reset()
        sword.reset()
    }
}

var g = Game()

for i in 1...100 {
    print("\n--round: \(i)--")
    g.hero.currentAction = (g.hero.Mp <= MP_CONSUMPTION) ? "pass" : "criticalAttack"
    g.boss.currentAction = (g.boss.Mp <= MP_CONSUMPTION) ? "pass" : "attack"
    g.sword.currentAction = "pass" //(g.sword.Mp <= MP_CONSUMPTION) ? "pass" : "dodge"
    if !g.proceed() { break }
    g.showStates()
    if (g.state == "over") {
        print("!! GAME IS OVER")
        break
    }
    g.reset()
}



