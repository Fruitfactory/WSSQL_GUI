/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.fruitfactory.pstriver.river;

import org.elasticsearch.common.inject.AbstractModule;
import org.elasticsearch.river.River;

/**
 *
 * @author Yariki
 */
public class PstRiverModule extends AbstractModule{

    @Override
    protected void configure() {
        bind(River.class).to(PstRiver.class).asEagerSingleton();
    }
    
    
    
    
}
