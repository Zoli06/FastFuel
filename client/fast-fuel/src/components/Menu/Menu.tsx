import React from 'react'
import { MenuLink } from './MenuLink'
import './Menu.css'

export const Menu = () => {
  return (
    <div className='menu'>
        <MenuLink text="Allergies" to="#" />
        <MenuLink text="Foods" to="#" />
        <MenuLink text="Ingredients" to="#" />
        <MenuLink text="Menus" to="#" />
        <MenuLink text="Orders" to="#" />
        <MenuLink text="Restaurants" to="#" />
        <MenuLink text="Stations" to="#" />
    </div>
  )
}
